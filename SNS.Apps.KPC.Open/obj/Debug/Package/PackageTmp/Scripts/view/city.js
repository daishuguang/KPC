(function () {
    var scrollposi = document.getElementById("citylist").offsetTop;
    window.addEventListener("scroll", function () {
        var documentscrolltop = document.body.scrollTop || document.documentElement.scrollTop;
        if (documentscrolltop >= window.innerHeight) {
            document.getElementById("top_back").style.display = "block";
        } else {
            document.getElementById("top_back").style.display = "none";
        }
    });

    document.getElementById("backbtn").addEventListener("click", function () {
        history.back();
    });

    document.addEventListener("DOMContentLoaded", function () {
        document.getElementById("letterlist").addEventListener("click", loadCityByLetter);
        document.getElementById("cities").addEventListener("click", setCity);
        document.getElementById("hotcity").addEventListener("click", setCity);
    });

    window.setCity = function (event) {
        var urlstr = window.location.search.split("&");
        if (urlstr.length === 7) {
            var citytype = urlstr[0].substr(urlstr[0].indexOf("=") + 1),
                urlreferrer = urlstr[1].substr(urlstr[1].indexOf("=") + 1),// create/search/edit
                fromcity = urlstr[2].substr(urlstr[2].indexOf("=") + 1),
                fromlocation = urlstr[3].substr(urlstr[3].indexOf("=") + 1),
                tocity = urlstr[4].substr(urlstr[4].indexOf("=") + 1),
                tolocation = urlstr[5].substr(urlstr[5].indexOf("=") + 1),
                routeguid = urlstr[6].substr(urlstr[6].indexOf("=") + 1),
                element = event.target;
            if (citytype === "fromcity") {
                fromcity = escape(element.innerText || element.textContent);
            } else {
                tocity = escape(element.innerText || element.textContent);
            }
            window.location.href = window.location.protocol + "//" + window.location.host + "/route/edit/" + routeguid + "?fromcity=" + fromcity + "&fromlocation=" + fromlocation + "&tocity=" + tocity + "&tolocation=" + tolocation;
        } else {
            if (urlstr.length === 1) {
                var ele = event.target,
                    currentcity = ele.innerText || ele.textContent;
                window.location.href = window.location.protocol + "//" + window.location.host + "/route/plaza?currentcity=" + currentcity;
            } else {
                var citytype = urlstr[0].substr(urlstr[0].indexOf("=") + 1),
                    urlreferrer = urlstr[1].substr(urlstr[1].indexOf("=") + 1),// create/search
                    element = event.target,
                    //sessionItem = window.sessionStorage.getItem(urlreferrer),
                    sessionItem = null,
                    whichtype = returnCookie(urlreferrer + "pintype");
                /*
                * 2014-03-26
                if (document.cookie.length > 0) {
                    var numstart = document.cookie.indexOf(urlreferrer + "="), numend = -1;
                    if (numstart != -1) {
                        numstart = numstart + urlreferrer.length + 1;
                        numend = document.cookie.indexOf(";", numstart);
                        if (numend !== -1)
                            sessionItem = document.cookie.substring(numstart, numend);
                        else
                            sessionItem = document.cookie.substring(numstart);
                    }
                }
                */
                //var cookieKey = urlreferrer + "_" + whichtype;
                var cookieKey = "city_" + whichtype;
                sessionItem = returnCookie(cookieKey);
                //var create_city = JSON.parse(sessionItem) || { fromcity: null, tocity: null };
                var create_city = (sessionItem === null ? null : JSON.parse(sessionItem)) || { fromcity: "", tocity: "" };
                //window.sessionStorage.removeItem(urlreferrer);
                create_city[citytype] = escape(element.innerText || element.textContent);
                //window.sessionStorage.setItem(urlreferrer, JSON.stringify(create_city));
                /*
                var now_Expires = new Date();
                now_Expires.setTime(now_Expires.getTime() - 1);
                document.cookie = urlreferrer + "=;expires=" + now_Expires.toGMTString();
                */
                //var now_Expires = new Date();
                //now_Expires.setTime(now_Expires.getTime() + 24 * 60 * 60 * 1000);
                document.cookie = cookieKey + "={\"fromcity\":\"" + create_city.fromcity + "\",\"tocity\":\"" + create_city.tocity + "\"};path=/;";
                if (citytype === "fromcity") {
                    document.cookie = "cityhistory=" + create_city["fromcity"] + ";path=/;";
                }
                //document.cookie = "cityhistory=" + ";path=/; expires=Sat, 01-Jan-2000 00:00:00 GMT";

                //window.localStorage.setItem(urlreferrer, JSON.stringify(create_city));
                //window.location.href = document.referrer;
                window.location.href = window.location.protocol + "//" + window.location.host + "/route/" + urlreferrer;
            }
        }
    }
    function returnCookie(searchstr) {
        var str = null;
        if (document.cookie.length > 0) {
            var numstart = document.cookie.indexOf(searchstr + "="), numend = -1;
            if (numstart != -1) {
                numstart = numstart + searchstr.length + 1;
                numend = document.cookie.indexOf(";", numstart);
                if (numend !== -1)
                    str = document.cookie.substring(numstart, numend);
                else
                    str = document.cookie.substring(numstart);
            }
        }
        return str;
    }

    function loadCityByLetter(letter) {
        letter = letter.target.innerText || letter.target.textContent;
        document.getElementById("h").innerHTML = letter;
        document.getElementById("shade").style.display = "block";
        document.getElementById("cities").style.display = "none";
        var xmlhttp = new XMLHttpRequest();
        if (window.XMLHttpRequest) {
            xmlhttp.onreadystatechange = function () {
                if (xmlhttp.readyState === 4 && xmlhttp.status === 200) {
                    var data = JSON.parse(xmlhttp.responseText);
                    var template_script = document.getElementById("city_template").innerHTML;
                    var template_HTML = _.template(template_script, { Big: letter, datas: data.cityList });
                    document.getElementById("cities").innerHTML = template_HTML;
                    document.getElementById("shade").style.display = "none";
                    document.getElementById("cities").style.display = "block";
                    window.scrollTo(0, scrollposi);
                }
            };
            xmlhttp.open("Get", "/api/loadcity?l=" + letter, true);
            xmlhttp.send(null);
        }
    }
})();