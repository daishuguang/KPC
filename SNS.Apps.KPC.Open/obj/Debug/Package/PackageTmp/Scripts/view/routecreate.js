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
                        //var $el = $("<li><a href='javascript:;' style='display:block'>" + result.district + result.street + result.streetNumber + result.business + "</a></li>");
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

    // navigator.geolocation.getCurrentPosition(callback)
    function updateLocation(position) {
        var latitude = position.coords.latitude;
        var longitude = position.coords.longitude;

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
                        var whichtype = "city_" + returnCookie("createpintype");
                        var sessionItem = returnCookie(whichtype),
                            create_city = JSON.parse(sessionItem) || { fromcity: "", tocity: "" };
                        create_city["fromcity"] = escape(add.city);
                        if ($("input:radio[name='pinchetype']:checked").val() === "sxb") {
                            create_city["tocity"] = add.city;
                            $("#tocity").val(add.city);
                        }
                        document.cookie = whichtype + "={\"fromcity\":\"" + create_city.fromcity + "\",\"tocity\":\"" + escape(create_city.tocity) + "\"};path=/;";
                        document.cookie = "cityhistory=" + create_city["fromcity"] + ";path=/;";
                        //$("#from_map").val(add.district + add.street + add.streetNumber);
                        $("#from_map").val(add.street + add.streetNumber);
                    }
                    if ($("#fromcity").val() === add.city && $("#from_map").val() === "") {
                        //$("#from_map").val(add.district + add.street + add.streetNumber);
                        $("#from_map").val(add.street + add.streetNumber);
                    }
					saveLocation();
                }
            });
        }
        BMap.Convertor.translate(ggPoint, 0, translateCallback);     //GCJ-02坐标转成百度坐标
    }

    if (navigator.geolocation && loadLocation) {
        navigator.geolocation.getCurrentPosition(updateLocation);
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
        //from.dispose();
        saveLocation();
    });

    /* 地图 */
    $(".mappop").on("click", function () {
        /*
        if (typeof (initialize) === "undefined") {
            document.getElementById("loadmap").src = "/bundles/route/maplocation?v=" + (new Date()).getTime();
        }
        */
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
        /*
        if (typeof (initialize) === "undefined") {
            document.getElementById("loadmap").onload = function () {
                initialize(flag);
            };
        } else {
            initialize(flag);
        }
        */
        initialize(flag);
    });

    $("#ok").on("click", function () {
        HideMap();
        //$("[data-role='footer']").fixedtoolbar("show");
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
            //$("#" + target).trigger("change");
        }
		saveLocation();
    });

    $("#cancel").on("click", HideMap);

    /* 选择 乘客/车主 */
    $("input:radio[name='UserRole']").on("change", function () {
        var $elid = $("input:radio[name='UserRole']:checked").attr("id");
        if ($elid === "radiodriver") {
            $("#driverstyle").css("display", "block");
            //$("#peoplecount").addClass("ct");
        } else {
            $("#driverstyle").css("display", "none");
            //$("#peoplecount").removeClass("ct");
        }
    });

    /* 车主基本信息 */
    $("#seatcount, #charge").on("keyup", function () {
        this.value = this.value.replace(/^0|\D/g, '');
    });

    /* 发布路线 */
    $("#btn_search").on("click", btn_search_touchstart_click);

    /* 设置正在处理的样式 */
    var el = document.getElementById("shade_top_div").style;
    window.addEventListener("scroll", function () {
        var documentscrolltop = document.body.scrollTop || document.documentElement.scrollTop;
        document.getElementById("shade_bottom").style.height = window.screen.height + documentscrolltop + "px";
        el.top = documentscrolltop + window.innerHeight / 2 + "px";
    });
    window.addEventListener("load", function () {
        el.width = window.innerWidth * 2 / 3 + "px";
        el.left = window.innerWidth / 2 - window.innerWidth / 3 + "px";
    });


    function HideMap() {
        $("#popupMap").css("display", "none");
        $("#wrapper").removeClass("hideback");
    }

    function btn_search_touchstart_click(event) {
        var flag = true;
        //var r1 = new RegExp(/^\+?[1-9][0-9]*$/);
        //$("input.mapvalue").each(function () {
        $("#fromcity, #tocity").each(function () {
            if ($.trim($(this).val()) === "") {
                flag = false;
                $(this).css("border", "1px solid red");
            }
        });

        if (document.getElementById("radiosxb").checked === true) {
            if ($.trim($("#from_map").val()) === "" || $.trim($("#to_map").val()) === "") {
                flag = false;
                alert("起点和终点必填,请输入详细地址");
                return;
            }
        }
        if ($("#fromcity").val() + $("#from_map").val() === $("#tocity").val() + $("#to_map").val()) {
            flag = false;
            alert("起点和终点不能相同");
            return;
        }
        if (flag) {
            $("#shade_bottom").css("display", "block");
            $("#shade_top_div").css("display", "block");
            $.when(wait1(), wait2())
                .done(function () {
                    $("#btn_search").prop("disabled", true);
                    saveLocation();
                    $("form").submit();
                })
                .fail(function () {
                    $("#btn_search").prop("disabled", false);
                })
                .always(function () {
                    $("#shade_bottom").css("display", "none");
                    $("#shade_top_div").css("display", "none");
                });
        }

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
                    alert("起点请填写" + $("#fromcity").val() + "内的地址");
                    dtd.reject();
                }
            });

            return dtd.promise();
        }

        function wait2() {
            var dtd = $.Deferred();
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
                    alert("终点请填写" + $("#tocity").val() + "内的地址");
                    dtd.reject();
                }
            });
            return dtd.promise();
        }

        event.preventDefault();
        return false;
    }
    function timeoutOperation(dtd) {
        dtd.reject();
        clearTimeout(timeout1);
        clearTimeout(timeout2);
        $("#shade_bottom").css("display", "none");
        $("#shade_top_div").css("display", "none");
        alert("您的网络不给力啊，请再提交一次");
    }
    // Prevent Enter keydown submit
    document.forms[0].addEventListener("keydown", function (e) {
        if (e.which === 13) {
            e.preventDefault();
            return false;
        }
    });

	var roles = ["rolepassenger", "roledriver"];
    for (var i = 0; i < roles.length; i++) {
        document.getElementById(roles[i]).addEventListener("click", function () {
            if (this.id !== roleselect) {
                document.getElementById(this.id).className = "selected";
                document.getElementById(roleselect).className = "";
                roleselect = this.id;
            }
        });
    }
    for (var i = 0; i < types.length; i++) {
        document.getElementById(types[i]).addEventListener("click", function () {
            if (this.id !== typeselect) {
                document.getElementById(this.id).className = "selected";
                document.getElementById(typeselect).className = "";
                typeselect = this.id;
            }
        });
    }
});

// Date&Time picker plugin
(function () {
    $("#StartDate_Date").mobiscroll().date({
        preset: "date",
        minDate: new Date(),
        theme: "default",
        lang: "zh",
        display: "bottom",
        dateOrder: "yyyy MM dd",
        mode: "scroller"
    });

    $("#startdate_date_temp").mobiscroll().date({
        preset: "date",
        minDate: new Date(),
        theme: "default",
        lang: "zh",
        display: "bottom",
        dateOrder: "yyyy MM dd",
        mode: "scroller"
    });

    $("#StartDate_Time").mobiscroll().time({
        preset: "time",
        theme: "default",
        lang: "zh",
        display: "bottom",
        mode: "scroller"
    });
})();

(function () {
    document.getElementById("mapdiv").style.height = (window.innerHeight - 50) + 'px';
    $("#btn_search").val("发布路线");
})();

(function () {
    $(document).ready(function () {
        $("input:radio[name='UserRole']").trigger("change");
    });
})();