(function () {
    function init() {
        // document.body.clientWidth vs document.documentElement.clientWidth
        document.getElementById("map_canvas").style.width = document.documentElement.clientWidth + "px";
        document.getElementById("map_canvas").style.height = document.documentElement.clientHeight + "px";
    }

    function window_load() {
        var markers = [],
            preTime = 0,
            preid = 0,
            previousCoordinate = null,
            el_shade_bottom = document.getElementById("shade_bottom"),
            el_shade_top = document.getElementById("shade_top_div"),
            MapOptions = {
                minZoom: 11 // 10公里 origin 12
            };

        function showShade(message) {
            $("#shade_top").empty().append(message);
            el_shade_bottom.style.display = "block";
            el_shade_top.style.display = "block";
            $("#btnclose").on("click", hideShade);
        }

        function hideShade() {
            el_shade_bottom.style.display = "none";
            el_shade_top.style.display = "none";
            $("#btnclose").off("click");
        }

        init();
        // 01 Create Map
        var map = new BMap.Map("map_canvas", MapOptions);

        var point = new BMap.Point(KPC.Around.user_point.lng, KPC.Around.user_point.lat);
        map.centerAndZoom(point, 15);

        // 02 addControl
        var opts1 = {
            offset: new BMap.Size(10, 50)
        };

        var opts2 = {
            type: BMAP_NAVIGATION_CONTROL_ZOOM
        };
        map.addControl(new BMap.ScaleControl(opts1));
        map.addControl(new BMap.NavigationControl(opts1));

        function load_centerMarker() {
            var myIcon01 = new BMap.Icon("/content/images/kpc_map_center.gif", new BMap.Size(24, 24), {
                offset: new BMap.Size(0, 0)
            });

            var marker_center = new BMap.Marker(point, { icon: myIcon01 });
            marker_center.setLabel(new BMap.Label("我的位置", {
                offset: new BMap.Size(0, -20)
            }));
            map.addOverlay(marker_center);
        }

        load_centerMarker();

        // 03 addOverlay
        var myIcon = new BMap.Icon("/content/images/kpc_around_marker.png", new BMap.Size(22, 39), {
            offset: new BMap.Size(0, 0)
        });

        // first page load
        $("#div_location").removeClass("hide");
        map_zoomend();
        map.addEventListener("zoomend", dragHandler);
        map.addEventListener("dragend", dragHandler);

        function dragHandler(e) {
            var nowTime = new Date().getTime();
            if (preTime == 0) {
                preTime = nowTime;
            }
            if ((nowTime - preTime) < 5000 && (nowTime - preTime) > 0) {
                clearTimeout(preid);
            } else {
                $("#div_location").removeClass("hide");
            }
            preTime = nowTime;
            preid = setTimeout(map_zoomend, 5000);
        }


        function addMarker(point, user, route) {
            var marker = new BMap.Marker(point, { icon: myIcon });
            var label = new BMap.Label(user.NickName, {
                offset: new BMap.Size(0, -20)
            });

            var info = (user.NickName == null) ? ("匿名用户") : (user.NickName);

            var message = "<a style='text-decoration:none;padding: 6px 0;background-color: #E8E9F7;display: block' href='http://" + window.location.host + "/user/viewprofile/" + user.UserGUID + "'>" + info + "<span style='float:right; color:green'>详细</span>" + "</a>" +
                          (
                            (typeof (route) == "undefined" || (typeof (route) != "undefined" && route == null)) ?
                            ("<p>未发布线路</p>") :
                            (
                            "<p>" + route.From_Location + "</p><p>" + route.To_Location + "</p>"
                            )
                          );
            marker.addEventListener("click", function () {
                showShade(message);
            });
            map.addOverlay(marker);
            label.setStyle({
                borderColor: "#808080",
                color: "#333"
            });
            markers.push(marker);
        }

        function loadAroundPoint(array_point) {
            (function () {
                for (var i = 0; i < array_point.length; i++) {
                    var point = new BMap.Point(array_point[i].Position.Longitude, array_point[i].Position.Latitude);
                    addMarker(point, array_point[i].User, array_point[i].Route);
                }
            })();
        }

        function map_zoomend() {
            var mapZone = map.getBounds(),
                mapZone_Southwest = mapZone.getSouthWest(),
                mapZone_NorthEast = mapZone.getNorthEast(),
                data_send = {
                    ld_lng: mapZone_Southwest.lng,
                    ld_lat: mapZone_Southwest.lat,
                    rt_lng: mapZone_NorthEast.lng,
                    rt_lat: mapZone_NorthEast.lat
                };

            $.ajax({
                async: true,
                url: "/api/around",
                type: "GET",
                data: data_send,
                dataType: "json",
                success: ajaxSuccess,
                error: ajaxError,
                complete: function () {
                    $("#div_location").addClass("hide");
                }
            });
        }

        function ajaxSuccess(data, textStatus, jqXHR) {
            map.clearOverlays();
            markers = [];
            load_centerMarker();
            loadAroundPoint(data.Data);
            var markerClusterer = new BMapLib.MarkerClusterer(map, { markers: markers });
        }

        function ajaxError(jqXHR, textStatus, errorThrown) {
            console.log(textStatus);
        }
    }

    if (window.attachEvent) {
        window.attachEvent("onload", window_load);
    } else {
        window.addEventListener("load", window_load);
    }
})();