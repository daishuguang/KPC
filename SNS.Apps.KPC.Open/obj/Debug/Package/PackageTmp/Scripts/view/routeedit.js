var myGeo, timeout1, timeout2;

$(document).ready(function () {
    var from = null,
        autooption = null,
        currentcity = null;

    // autocomplete
    try {
        var map = new BMap.Map("mapdiv");
    } catch (e) {
        window.location.reload();
    }
    map.centerAndZoom("上海", 12);
    var myGeo = new BMap.Geocoder();

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
                        //var $el = $("<li><a href='javascript:;' style='display:block'>" + result.district + result.street + result.streetNumber + result.business + "</li>");
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

    /* 始发地/目的地 */
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

    $("#btn_address, #backbtn2").on("click", function () {
        $("#address_text").trigger("change");
        $("#address").addClass("hideback").removeClass("zindex");
        $("#wrapper").removeClass("hideback").addClass("zindex");
        $("#suggest").empty();
        var address_text = $("#address_text").val();
        $("#" + $("#address_text").data("control")).val(address_text);
        //from.dispose();
    });


    $("#btn_submit").on("click", function () {
        if (KPC.Tongcheng === "true") {
            if ($.trim($("#from_map").val()) === "" || $.trim($("#to_map").val()) === "") {
                flag = false;
                hideWait();
                alert("起点和终点必填,请输入详细地址");
                return;
            }
        }
        if ($("#fromcity").val() + $("#from_map").val() === $("#tocity").val() + $("#to_map").val()) {
            flag = false;
            hideWait();
            alert("起点和终点不能相同");
            return;
        }

        $.when(wait1(), wait2())
        .done(function () {
            document.forms[0].submit();
        })
        .fail(function () {
            hideWait();
        });
        //.fail(timeoutOperation);
    });

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

    function timeoutOperation(dtd) {
        if (typeof (dtd) !== "undefined") {
            dtd.reject();
        }
        hideWait();
        alert("您的网络不给力啊，请再提交一次");
    }

    /* 车主基本信息 */
    $("#seatcount, #charge").on("keyup", function () {
        this.value = this.value.replace(/^0|\D/g, '');
    });

    // Prevent Enter keydown submit
    document.forms[0].addEventListener("keydown", function (e) {
        if (e.which === 13) {
            e.preventDefault();
            return false;
        }
    });

    document.getElementById("fromcity_div").addEventListener("click", function () {
        window.location.href = window.location.protocol + "//" + window.location.host + "/route/city?citytype=fromcity&referrer=edit&fromcity=" + escape($("#fromcity").val()) + "&fromlocation=" + escape($("#from_map").val()) + "&tocity=" + escape($("#tocity").val()) + "&tolocation=" + escape($("#to_map").val()) + "&routguid=" + KPC.RouteEdit.RouteGuid;
    });

    document.getElementById("tocity_div").addEventListener("click", function () {
        window.location.href = window.location.protocol + "//" + window.location.host + "/route/city?citytype=tocity&referrer=edit&fromcity=" + escape($("#fromcity").val()) + "&fromlocation=" + escape($("#from_map").val()) + "&tocity=" + escape($("#tocity").val()) + "&tolocation=" + escape($("#to_map").val()) + "&routguid=" + KPC.RouteEdit.RouteGuid;
    });

    document.getElementById("backbtn").addEventListener("click", function () {
        //history.back();
        window.location.href = "/route/list";
    });

    var tongcheng = [], callback;
    if (KPC.RouteEdit.RouteType === "sxb") {
        tongcheng = [["workday", "none", "70%"],
                    ["twoday", "none", "70%"],
                    ["tempday", "", "35%"],
                    ["eachday", "none", "70%"]];
        callback = funcSxb;
    } else {
        tongcheng = [["workday", "search ct", ""],
                    ["twoday", "search ct", ""],
                    ["tempday", "search", ""],
                    ["eachday", "search ct", ""]];
        callback = funcCt;
    }
    for (var i = 0; i < tongcheng.length; i++) {
        (function (i) {
            document.getElementById(tongcheng[i][0]).addEventListener("click", function () { callback(tongcheng[i][1], tongcheng[i][2]); });
        })(i);
    }

    function funcSxb(style, width) {
        document.getElementById("StartDate_Time").style.width = width;
        document.getElementById("startdate_date_temp").style.display = style;
    }

    function funcCt(style, width) {
        document.getElementById("ct_date").className = style;
    }

});

// Date&Time picker plugin
(function () {
    if (document.getElementById("StartDate_Date") !== null) {
        bindDate("StartDate_Date");
    } else {
        bindDate("startdate_date_temp");
    }

    function bindDate(eleid) {
        $("#" + eleid).mobiscroll().date({
            preset: "date",
            minDate: new Date(),
            theme: "default",
            lang: "zh",
            display: "bottom",
            dateOrder: "yyyy MM dd",
            mode: "scroller"
        });
    }

    $("#StartDate_Time").mobiscroll().time({
        preset: "time",
        theme: "default",
        lang: "zh",
        display: "bottom",
        mode: "scroller"
    });
})();