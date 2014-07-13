var globals, locationPoint, coordFrom, coordTo, coordinate;

$(document).ready(function () {

    // autocomplete
    var map = new BMap.Map("mapdiv");
    map.centerAndZoom("上海", 12);
    var myGeo = new BMap.Geocoder();

    var autooption = {
        input: "address_text",
        location: map,
        onSearchComplete: function (AutocompleteResult) {
            from.hide();
            $("#suggest").empty();
            var num = AutocompleteResult.getNumPois();
            (function () {
                for (var i = 0; i < num; i++) {
                    var result = AutocompleteResult.getPoi(i);
                    var $el = $("<li><a href='javascript:;' style='display:block'>" + result.city + result.district + result.street + result.streetNumber + result.business + "</li>");
                    $el.appendTo("#suggest").on("click", func_suggest);
                    //$("#suggest").append("<li>" + result.city + result.district + result.street + result.streetNumber + result.business + "</li>");
                }
            })();
        }
    };

    var from = new BMap.Autocomplete(autooption);
    /*
	if (document.hasOwnProperty("ontouchend")) {
		$("#suggest").on("touchend", func_suggest);
	}
	else {
		$("#suggest").on("click", func_suggest);
	}
    */
    function func_suggest(e) {
        $("#address_text").val($(e.target).html());
        $("#btn_address").trigger("click");
    }

    // navigator.geolocation.getCurrentPosition(callback)
    function updateLocation(position) {

        var latitude = position.coords.latitude;

        var longitude = position.coords.longitude;


        if (!latitude || !longitude) {

            /*

            document.getElementById("div_geolocation").innerText = "定位失败";

            window.setTimeout(function () {
                document.getElementById("div_geolocation").style.display = "none";
            }, 5000);
            */
            return;

        }
        var crd = {
            "longitude": longitude,
            "latitude": latitude
        };

        //locationPoint = crd;

        // change below
        var ggPoint = new BMap.Point(longitude, latitude);

        translateCallback = function (point) {
            crd = {
                "longitude": point.lng,
                "latitude": point.lat
            };

            locationPoint = crd;
            /*
            document.getElementById("div_geolocation").innerHTML = "定位成功";

            window.setTimeout(function () {
                $("#div_geolocation").fadeOut("slow");
            }, 2000);
            */
            // modified at 2013/

            // getLoaction
            myGeo.getLocation(point, function (result) {
                if (result) {
                    var add = result.addressComponents;
                    if ($("#from_map").val() == "") {
                        $("#from_map").val(result.address);
                    }
                }
            });

            // ReCenterandZoom
            window.frames["map"].initialize();

        }

        setTimeout(function () {
            BMap.Convertor.translate(ggPoint, 0, translateCallback);     //GCJ-02坐标转成百度坐标
        }, 5000);
    }

    function showError(error) {
        /*
        switch (error.code) {
            case 1:
                document.getElementById("div_geolocation").innerText = "定位失败,您未允许浏览器定位";
                break;
            default:
                document.getElementById("div_geolocation").innerText = "定位失败";
                break;
        }
        window.setTimeout(function () {
            $("#div_geolocation").fadeOut("slow");
        }, 2000);
        */
    }

    function loadDemo() {

        if (navigator.geolocation) {

            navigator.geolocation.getCurrentPosition(updateLocation, showError);

        } else {
            /*
            document.getElementById("div_geolocation").innerHTML = "您的设备不支持定位";
            window.setTimeout(function () {
                document.getElementById("div_geolocation").style.display = "none";
            }, 2000);
            */
        }

    }

    loadDemo();

    // validate the input and remove border style
    $("input[id!='address_text']").on("change", function () {
        var r1 = new RegExp(/^\+?[1-9][0-9]*$/);
        if ($.trim($(this).val()) == "") {
            $(this).css("border", "1px solid red");

            // hide charge_error
            if (this.id == "Charge") {
                $("#charge_error").css("display", "none");
            }

            // hide seat_error
            if (this.id == "SeatCount") {
                $("#seat_error").css("display", "none");
            }
        } else {
            $(this).css("border", "");
            if (this.id == "Charge") {
                var flag = r1.test($(this).val());
                if (!flag) {
                    $("#charge_error").css("display", "block");
                    $(this).css("border", "1px solid #FADB4E");
                } else {
                    $("#charge_error").css("display", "none");
                    $(this).css("border", "");
                }
            }

            if (this.id == "SeatCount") {
                var flag2 = r1.test($(this).val());
                if (!flag2) {
                    $(this).css("border", "1px solid red");
                    $("#seat_error").css("display", "block");
                } else {
                    $(this).css("border", "");
                    $("#seat_error").css("display", "none");
                }
            }
        }

        if (this.id == "Charge") {
            if ($.trim($(this).val()) != "") {
                var flag = r1.test($(this).val());
                if (!flag)
                    $(this).css("border", "1px solid red");
            } else {

            }
        }
        if (this.id == "SeatCount") {
            var flag2 = r1.test($(this).val());
            if (!flag2)
                $(this).css("border", "1px solid red");
        }
    });

    // Display:block : display:none
    $("input:radio").on("change", function () {
        var $elid = $("input:radio:checked").attr("id");
        if ($elid === "findpassenger") {
            $("#role_text").text("我是车主");
            $(".driverstyle").css("display", "block");
            $("#lblCharge").text("拼车费");
        } else {
            $("#role_text").text("我是乘客");
            $(".driverstyle").css("display", "none");
            $("#lblCharge").text("出价");
        }
        $("#seat_error").css("display", "none");
        $("input").css("border", "");
    });

    // popup

    if (document.hasOwnProperty("ontouchstart")) {
        $(".mappop").on("touchstart", mappop_touchstart_click);
    }
    else {
        $(".mappop").on("click", mappop_touchstart_click);
    }

    $("#address_text").on("change", function () {
        if ($(this).data("control") == "from_map") {
            coordFrom = null;
        } else {
            coordTo = null;
        }
    });

    function mappop_touchstart_click() {
        $("#mainList").addClass("hideul");

        $("#popupMap").css("display", "block").data({
            "target": $(this).attr("id") + "_map"
        });

        $("#popupMap").parent().parent(".ui-content").removeClass("ui-content");

        // initialize centerZoom of Map
        globals = $("#" + $(this).attr("id") + "_map").val();
        if (this.id == "from") {
            coordinate = coordFrom;
        }
        if (this.id == "to") {
            coordinate = coordTo;
        }
        window.frames["map"].initialize();
    }

    function HideMap() {
        $("#popupMap").css("display", "none");
        $("#mainList").removeClass("hideul");
        $("#popupMap").parent().parent("div").addClass("ui-content");
    }

    /*
    $("#from_map, #to_map").on("focus", function () {
        var $offset = $(this).offset();
        var $offsetTop = $offset.top;
        var $offsetLeft = $offset.left;

        window.scrollTo($offsetLeft, $offsetTop);
    });
    */

    // Get lng, lat after input change, assignment to hidden input
    function ValidateMap($el) {
        var el = $el.attr("id");

        myGeo.getPoint($el.val(), function (point) {
            if (point === null) {
                $("#" + el).val("");
                $("#" + el).attr("placeholder", "无法找到地址，可以在地图上点选");
                return false;
            }
            else {

                if (el == "from_map") {
                    $("#from_lng").val(point.lng);
                    $("#from_lat").val(point.lat);
                } else if (el == "to_map") {
                    $("#to_lng").val(point.lng);
                    $("#to_lat").val(point.lat);
                }
                return true;
            }
        });
    }

    // Search btn to validate the input
    if (document.hasOwnProperty("ontouchend")) {
        $("#btn_search").on("touchend", btn_search_touchstart_click);
        $("#btn_search").on("click", function (event) {
            event.preventDefault();
        });
    } else {
        $("#btn_search").on("click", btn_search_touchstart_click);
    }

    function btn_search_touchstart_click(event) {
        var flag = true;
        var r1 = new RegExp(/^\+?[1-9][0-9]*$/);

        if ($("input:radio:checked").val() === "Driver") {
            flag = true;
            $("input.mapvalue").each(function () {
                if ($.trim($(this).val()) == "") {
                    flag = false;
                    $(this).css("border", "1px solid red");
                }
            });

            // charge validate
            if ($.trim($("#Charge").val()) != "") {
                var flag2 = r1.test($("#Charge").val());
                if (!flag2) {
                    flag = false;
                    $("#Charge").css("border", "1px solid red");
                    $("#charge_error").css("display", "block");
                } else {
                    $("#charge_error").css("display", "none");
                }
            } else {
                flag = false;
                $("#Charge").css("border", "1px solid red");
                $("#charge_error").css("display", "none");
            }

            // seat validate Isempty ? false : ""
            if ($.trim($("#SeatCount").val()) != "") {
                var flag3 = r1.test($("#SeatCount").val());
                if (!flag3) {
                    flag = false;
                    $("#SeatCount").css("border", "1px solid red");
                    $("#seat_error").css("display", "block");
                } else {
                    $("#seat_error").css("display", "none");
                }
            } else {
                flag = false;
                $("#SeatCount").css("border", "1px solid red");
                $("#seat_error").css("display", "none");
            }

        } else {
            flag = true;
            // validate the from_map, to_map
            $("input.mapvalue").each(function () {
                if ($.trim($(this).val()) == "") {
                    flag = false;
                    $(this).css("border", "1px solid red");
                }
            });

            // charge validate
            if ($.trim($("#Charge").val()) != "") {
                var flag2 = r1.test($("#Charge").val());
                if (!flag2) {
                    flag = false;
                    $("#Charge").css("border", "1px solid red");
                    $("#charge_error").css("display", "block");
                } else {
                    $("#charge_error").css("display", "none");
                }
            }
        }

        // submit
        if (flag) {
            $("#shade_bottom").css("display", "block");
            $("#shade_top_div").css("display", "block");
            if (coordFrom) {
                (function () {
                    var pt = new BMap.Point(coordFrom.longitude, coordFrom.latitude);
                    myGeo.getLocation(pt, function (rs) {
                        var from_address = rs.addressComponents;
                        $("#from_province").val(from_address.province);
                        $("#from_city").val(from_address.city);
                        $("#from_district").val(from_address.district);
                        $("#from_lng").val(coordFrom.longitude);
                        $("#from_lat").val(coordFrom.latitude);
                        Geo_tomap();
                    });
                })();
            } else {
                myGeo.getPoint($("#from_map").val(), function (point) {
                    if (point === null) {
                        $("#from_map").val("");
                        $("#from_map").attr("placeholder", "无法找到地址，可以在地图上点选");
                        flag = false;
                    }
                    else {
                        (function () {
                            myGeo.getLocation(point, function (rs) {
                                var from_address = rs.addressComponents;
                                $("#from_province").val(from_address.province);
                                $("#from_city").val(from_address.city);
                                $("#from_district").val(from_address.district);
                                $("#from_lng").val(point.lng);
                                $("#from_lat").val(point.lat);
                                Geo_tomap();
                            });
                        })();
                    }
                });
            }
        }

        function Geo_tomap() {
            if (coordTo) {
                (function () {
                    var pt = new BMap.Point(coordTo.longitude, coordTo.latitude);
                    myGeo.getLocation(pt, function (rs) {
                        var to_address = rs.addressComponents;
                        $("#to_province").val(to_address.province);
                        $("#to_city").val(to_address.city);
                        $("#to_district").val(to_address.district);
                        $("#to_lng").val(coordTo.longitude);
                        $("#to_lat").val(coordTo.latitude);
                        if (flag) {
                            $("#btn_search").prop("disabled", true);
                            $("form").submit();
                            $("#shade_bottom").css("display", "none");
                            $("#shade_top_div").css("display", "none");
                        }
                    });
                })();
            }
            else {
                myGeo.getPoint($("#to_map").val(), function (point) {
                    if (point === null) {
                        $("#to_map").val("");
                        $("#to_map").attr("placeholder", "无法找到地址，可以在地图上点选");
                        flag = false;
                    }
                    else {
                        (function () {
                            myGeo.getLocation(point, function (rs) {
                                var to_address = rs.addressComponents;
                                $("#to_province").val(to_address.province);
                                $("#to_city").val(to_address.city);
                                $("#to_district").val(to_address.district);
                                $("#to_lng").val(point.lng);
                                $("#to_lat").val(point.lat);
                                if (flag) {
                                    $("#btn_search").prop("disabled", true);
                                    $("form").submit();
                                    $("#shade_bottom").css("display", "none");
                                    $("#shade_top_div").css("display", "none");
                                }
                            });
                        })();
                    }
                });
            }
        }
        event.preventDefault();
        return false;
    }

    function ok_touchstart_click() {
        HideMap();
        //$("[data-role='footer']").fixedtoolbar("show");
        if (globals != null && globals !== "") {
            $("#" + $("#popupMap").data("target")).val(globals).css("border", "");
            var target = $("#popupMap").data("target");
            if (target == "from_map") {
                if (coordinate) {
                    coordFrom = coordinate;
                    coordinate = null;
                }
            }
            if (target == "to_map") {
                if (coordinate) {
                    coordTo = coordinate;
                    coordinate = null;
                }
            }
            //$("#" + target).trigger("change");
        }
    }

    // Cancel button
    $("#ok").on("click", ok_touchstart_click);
    $("#cancel").on("click", HideMap);
    $("#btn_address").on("click", func_btn_confirm);

    function func_btn_confirm() {
        $("#address_text").trigger("change");
        $("#address").addClass("hideul").removeClass("zindex");
        $("#mainList").removeClass("hideul").addClass("zindex");
        $("#suggest").empty();
        $("#" + $("#address_text").data("control")).val($("#address_text").val());
    }

    $(".mapvalue").on("change", function () {
        if (this.id == "from_map") {
            coordFrom = null;
        }
        if (this.id == "to_map") {
            coordTo = null;
        }
    });

    $(".mapvalue").on("focus", function () {
        $("#address").removeClass("hideul").addClass("zindex");
        $("#mainList").addClass("hideul").removeClass("zindex");
        $("#address_text").focus();
        $("#address_text").val(this.value).data("control", $(this).attr("id"));
    });

    // Prevent Enter keydown submit
    function Prevent_submit() {
        if (window.attachEvent) {
            document.forms[0].attachEvent("onkeydown", function (e) {
                if (e.keyCode == 13) {
                    e.returnValue = true;
                    return false;
                }
            });
        } else {
            document.forms[0].addEventListener("keydown", function (e) {
                if (e.which == 13) {
                    e.preventDefault();
                    return false;
                }
            });
        }
    }

    Prevent_submit();

    $("#repeat_text").parent("span").on("click", function () {
        $("#repeat_select").mobiscroll("show");
    });

    $("#role_text").parent("span").on("click", function () {
        $("#UserRole").mobiscroll("show");
    });

    $("#repeat_select").on("change", function () {
        var $option = $("#repeat_select option");
        var $length = $option.length;
        var txt = "";
        var IsSelected = false;
        for (var k = 0; k < $length; k++) {
            if ($option[k].selected) {
                IsSelected = true;
                txt += $option[k].text + " ";
            }
        }
        if (IsSelected) {
            $("#repeat_text").text(txt);
            $("#repeat_endtime").css("display", "block");
        } else {
            $("#repeat_text").text("单  次");
            $("#repeat_endtime").css("display", "none");
        }
    });
});

(function () {
    $(document).ready(function () {
        $("input:radio").trigger("change");
    });
})();

(function () {
    $("#StartDate_Date").on("click", function () {
        $('#calendar1').fadeIn();
    });

    var $calendar1 = $('#calendar1').hide();

    var gCalendar1 = new Calendar();

    gCalendar1.ready($calendar1);

    $calendar1.on('startDateChanged', function () {

        var $this = $(this);
        var startdate = $this.attr('data-startdate');

        var checkin = moment(new Date(startdate));

        // 如果入住日期大于等于离店日期，修改离店日期为入住日期的第二天
        $("#StartDate_Date").val(checkin.format('YYYY-MM-DD'));
        $('#calendar1').fadeOut();

    });
})();

(function () {
    $("#baidumap").css("height", function () {
        return ($(window).height() - 45) + "px";
    });

    $("#btn_search").prop("disabled", false);
    $("#btn_search_txt").text("发布路线");
})();