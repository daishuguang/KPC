var map, globals, globals_currentcity, locationPoint, latestAddress = null, myGeo, coordFrom, coordTo, coordinate, timeout1, timeout2;

$(document).ready(function () {
    var from = null,
        autooption = null,
        currentcity = null;

    // autocomplete
    try {
        map = new BMap.Map("mapdiv");
    } catch (e) {
        window.location.reload();
    }

    map.centerAndZoom("上海", 12);
    myGeo = new BMap.Geocoder();

    autooption = {
        input: "address_text",
        location: map,
        onSearchComplete: function (AutocompleteResult) {
            from.hide();//隐藏baidu的提示
            $("#suggest").empty();
            var num = AutocompleteResult.getNumPois();
            (function () {
                for (var i = 0; i < num; i++) {
                    var result = AutocompleteResult.getPoi(i);
                    if (result.city === currentcity) {
                        var $el = $("<li><a href='javascript:;' style='display:block'>" + result.street + result.streetNumber + result.business + "</a>" + "<span style='color:gray;position:absolute;z-index:-1;right:0;top:0;padding:1em 10px;'>" + result.district + "</span></li>");
                        $el.appendTo("#suggest").on("click", function (e) {
                            $("#address_text").val($(e.target).html());
                            $("#suggest").empty();
                            $("#btn_address").trigger("click");
                        });
                    }
                }
            })();
        }
    };

    from = new BMap.Autocomplete(autooption);

    /* 定位 */
    if (navigator.geolocation && loadLocation) {
        console.log("geolocation");
        navigator.geolocation.getCurrentPosition(updateLocation);
    }

    // navigator.geolocation.getCurrentPosition(callback)
    function updateLocation(position) {
        var latitude = position.coords.latitude,
            longitude = position.coords.longitude;

        if (!latitude || !longitude) {
            return;
        }

        var crd = {
            "longitude": longitude,
            "latitude": latitude
        };

        // change below
        var ggPoint = new BMap.Point(longitude, latitude);

        translateCallback = function (point) {
            crd = {
                "longitude": point.lng,
                "latitude": point.lat
            };

            locationPoint = crd;

            // getLoaction
            myGeo.getLocation(point, function (result) {
                if (result) {
                    var add = result.addressComponents;
                    if ($("#fromcity").val() === "") {
                        $("#fromcity").val(add.city).css("border", "");
                        var whichtype = "city_" + returnCookie("searchpintype");
                        var sessionItem = returnCookie(whichtype),
                            create_city = JSON.parse(sessionItem) || { fromcity: "", tocity: "" };
                        create_city["fromcity"] = escape(add.city);
                        //var now_Expires = new Date();
                        //now_Expires.setTime(now_Expires.getTime() + 24 * 60 * 60 * 1000);
                        //document.cookie = whichtype + "={\"fromcity\":\"" + create_city.fromcity + "\",\"tocity\":\"" + create_city.tocity + "\"};path=/;expires=" + now_Expires.toGMTString();
                        document.cookie = whichtype + "={\"fromcity\":\"" + create_city.fromcity + "\",\"tocity\":\"" + create_city.tocity + "\"};path=/;";
                        document.cookie = "cityhistory=" + create_city["fromcity"] + ";path=/;";
                        $("#from_map").val(add.street + add.streetNumber);
                    }
                    if ($("#fromcity").val() === add.city && $("#from_map").val() === "") {
                        $("#from_map").val(add.street + add.streetNumber);
                    }
                }
            });
        }
        BMap.Convertor.translate(ggPoint, 0, translateCallback);     //GCJ-02坐标转成百度坐标
    }

    /* 始发地/目的地 */
    $(".mapvalue").on("change", function () {
        if (this.id === "from_map") {
            coordFrom = null;
        }
        if (this.id === "to_map") {
            coordTo = null;
        }
    });

    $(".mapvalue").on("focus", function () {
        currentcity = null;
        if (this.id === "from_map") {
            currentcity = $("#fromcity").val();
        } else {
            currentcity = $("#tocity").val();
        }
        $("#cityown").text(currentcity);
        currentcity = currentcity || map;
        from.setLocation(currentcity);
        $("#address").removeClass("hideback").addClass("zindex");
        $("#wrapper").addClass("hideback").removeClass("zindex");
        $("#address_text").data("control", $(this).attr("id")).focus().val(this.value);
    });

    $("#address_text").on("change", function () {
        if ($(this).data("control") === "from_map") {
            coordFrom = null;
        } else {
            coordTo = null;
        }
    });

    $("#btn_address, #backbtn").on("click", function () {
        $("#address_text").trigger("change");
        $("#address").addClass("hideback").removeClass("zindex");
        $("#wrapper").removeClass("hideback").addClass("zindex");
        $("#suggest").empty();
        var address_text = $("#address_text").val();
        $("#" + $("#address_text").data("control")).val(address_text);
        saveLocation();
    });

    /* 地图点选 */
    $(".mappop").on("click", function () {
        var flag = null;
        $("#wrapper").addClass("hideback");

        $("#popupMap").css("display", "block").data({
            "target": $(this).attr("id") + "_map"
        });

        // initialize centerZoom of Map
        globals = $("#" + $(this).attr("id") + "_map").val();
        globals_currentcity = $("#" + $(this).attr("id") + "city").val();
        if (this.id === "from") {
            coordinate = coordFrom;
        }
        if (this.id === "to") {
            coordinate = coordTo;
            flag = true;
        }
        initialize(flag);
    });
    /* 确定 */
    $("#ok").on("click", function () {
        HideMap();
        if (globals != null && globals !== "") {
            $("#" + $("#popupMap").data("target")).val(globals).css("border", "");
            var target = $("#popupMap").data("target");
            if (target === "from_map") {
                if (coordinate) {
                    coordFrom = coordinate;
                    coordinate = null;
                }
            }
            if (target === "to_map") {
                if (coordinate) {
                    coordTo = coordinate;
                    coordinate = null;
                }
            }
        }
    });

    $("#cancel").on("click", HideMap);

    function HideMap() {
        $("#popupMap").css("display", "none");
        $("#wrapper").removeClass("hideback");
    }

    /* 搜索路线 */
    $("#btn_search").on("click", function () {
        var flag = true;
        if ($("#fromcity").val() === "") {
            flag = false;
            $("#fromcity").css("border", "1px solid red");
        }
        if (flag) {
            //$("#shade").css("display", "block");// 正在加载中...
            showWait();
            $.when(wait1(), wait2())
                .done(function () {
                    saveLocation();
                    $("form").submit();
                })
            .fail(function () {
                //$("#shade").css("display", "none");
                hideWait();
            });
        }
    });

    /* baidu获取from */
    function wait1() {
        var dtd = $.Deferred();
        timeout1 = setTimeout(function () { timeoutOperation(dtd); }, 10000);
        myGeo.getPoint($("#fromcity").val() + $("#from_map").val(), function (point) {
            if (point) {
                myGeo.getLocation(point, function (rs) {
                    clearTimeout(timeout1);
                    var from_address = rs.addressComponents;
                    $("#from_province").val(from_address.province);
                    $("#from_district").val(from_address.district);
                    $("#from_lng").val(point.lng);
                    $("#from_lat").val(point.lat);
                    dtd.resolve();
                });
            } else {
                clearTimeout(timeout1);
                dtd.resolve();
            }
        });
        return dtd.promise();
    }

    /* baidu获取to */
    function wait2() {
        var dtd = $.Deferred();
        if ($("#tocity").val() + $("#to_map").val() !== "") {
            timeout2 = setTimeout(function () { timeoutOperation(dtd); }, 10000);
            myGeo.getPoint($("#tocity").val() + $("#to_map").val(), function (point) {
                if (point) {
                    myGeo.getLocation(point, function (rs) {
                        clearTimeout(timeout2);
                        var to_address = rs.addressComponents;
                        $("#to_province").val(to_address.province);
                        $("#to_district").val(to_address.district);
                        $("#to_lng").val(point.lng);
                        $("#to_lat").val(point.lat);
                        dtd.resolve();
                    });
                } else {
                    clearTimeout(timeout2);
                    dtd.resolve();
                }
            });
        } else {
            dtd.resolve();
        }
        return dtd.promise();
    }

    function timeoutOperation(dtd) {
        dtd.reject();
        //$("#shade").css("display", "none");
        clearTimeout(timeout1);
        clearTimeout(timeout2);
        hideWait();
        alert("您的网络不给力啊，请再提交一次");
    }

    /*
    var startX = null, stopX = null;
    document.documentElement.addEventListener("touchstart", function (e) {
        e.stopPropagation();
        startX = e.touches[0].pageX;
    });
    document.documentElement.addEventListener("touchmove", function (e) {
        e.stopPropagation();
        stopX = e.touches[0].pageX;
    });
    document.documentElement.addEventListener("touchend", function (e) {
        e.stopPropagation();
        if (startX - stopX > 50) {
            document.getElementById("nav").style.display = "none";
            document.getElementById("wrapper").className = "";
            document.documentElement.style.position = "";
        }
        if (stopX - startX > 50) {
            document.getElementById("nav").style.display = "block";
            document.getElementById("wrapper").className = "nav";
            document.documentElement.style.position = "fixed";
        }
    });
    */
    function dateselect() {

    }
    var roles = ["lblall", "lblpassenger", "lbldriver"];
    roleselect();
    var dates = ["lbldateall", "lbldateam", "lbldatepm"];
    dateselect();

    function dateselect() {
        for (var i = 0; i < dates.length; i++) {
            document.getElementById(dates[i]).addEventListener("click", function () {
                if (this.id !== dateselected) {
                    document.getElementById(this.id).className = "selected";
                    document.getElementById(dateselected).className = "";
                    dateselected = this.id;
                }
            });
        }
    }
    function roleselect() {
        for (var i = 0; i < roles.length; i++) {
            document.getElementById(roles[i]).addEventListener("click", function () {
                if (this.id !== roleselected) {
                    document.getElementById(this.id).className = "selected";
                    document.getElementById(roleselected).className = "";
                    roleselected = this.id;
                }
            });
        }
    }

    document.getElementById("mapdiv").style.height = (window.innerHeight - 50) + 'px';
});

// Date&Time picker plugin
(function () {
    $("#StartDate_Date").mobiscroll().date({
        preset: "date",
        theme: "default",
        lang: "zh",
        display: "bottom",
        dateOrder: "yyyy MM dd",
        mode: "scroller"
    });
})();
