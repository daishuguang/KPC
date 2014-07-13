(function () {
    // autocomplete
    try {
        map = new BMap.Map("mapdiv");
    } catch (e) {
        window.location.reload();
    }
    map.centerAndZoom("上海", 12);
    myGeo = new BMap.Geocoder();

    if (navigator.geolocation) {
        setTimeout(function () { document.getElementById("geolocation").innerText = "请点击屏幕左下角的键盘图标，然后点击【+】图标，选择【位置】，最后点击【发送】按钮提交您的地理位置。"; }, 8000);
        navigator.geolocation.getCurrentPosition(updateLocation, showError);
    }

    function showError(error) {
        switch (error.code) {
            case error.PERMISSION_DENIED:
                console.log(error.PERMISSION_DENIED);
            case error.POSITION_UNAVAILABLE:
                console.log(error.POSITION_UNAVAILABLE);
            case error.TIMEOUT:
                console.log(error.TIMEOUT);
            case error.UNKNOWN_ERROR:
                console.log(error.UNKNOWN_ERROR);
            default:
                document.getElementById("geolocation").innerText = "请点击屏幕左下角的键盘图标，然后点击【+】图标，选择【位置】，最后点击【发送】按钮提交您的地理位置。";
                break;
        }
    }

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

        console.log(crd);

        // change below
        var ggPoint = new BMap.Point(longitude, latitude);

        translateCallback = function (point) {
            crd = {
                "longitude": point.lng,
                "latitude": point.lat
            };
            console.log(crd);
            var xhr;
            if (window.XMLHttpRequest) {
                xhr = new XMLHttpRequest();
                xhr.onreadystatechange = function () {
                    if (xhr.readyState === 4 && xhr.status === 200) {
                        console.log(xhr.responseText);
                        var data = JSON.parse(xhr.responseText);
                        if (data.Status === 0) {
                            window.location.replace(location.href);
                        }
                        else {
                            console.log("失败");
                        }
                    }
                };
                var url = "/api/setusertrack?lng=" + crd.longitude + "&lat=" + crd.latitude;
                xhr.open("GET", url, null);
                xhr.send();
            }
        }
        BMap.Convertor.translate(ggPoint, 0, translateCallback);     //GCJ-02坐标转成百度坐标
    }


    document.getElementById("navbar").addEventListener("click", function (e) {
        if (document.documentElement.style.position !== "fixed") {
            document.getElementById("nav").style.display = "block";
            document.getElementById("wrapper").className = "nav";
            document.documentElement.style.position = "fixed";
        } else {
            document.body.click();
        }
        e.stopPropagation();
    });

    document.body.addEventListener("click", function () {
        if (document.getElementById("nav").style.display === "block") {
            document.getElementById("nav").style.display = "none";
            document.getElementById("wrapper").className = "";
            document.documentElement.style.position = "";
        }
    });
})();