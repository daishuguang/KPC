(function () {
    var IsSendRequest = true, num = 0, hashNum = 0, flag = false;
    document.addEventListener("DOMContentLoaded", function () {
        if (location.hash) {
            flag = true;
            hashNum = location.hash.substr(1);
            searchResult();
        }
    });

    /*
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
    */

    document.getElementById("backbtn").addEventListener("click", function () {
        if (location.hash) {
            window.location.href = "/route/search";
        } else
            history.back();
    });

    document.body.addEventListener("click", function () {
        if (document.getElementById("nav").style.display === "block") {
            document.getElementById("nav").style.display = "none";
            document.getElementById("wrapper").className = "";
            document.documentElement.style.position = "";
        }
    });

    /* 上拉加载更多 */
    window.addEventListener("scroll", function () {

        //var documentscrollheight = document.body.scrollHeight || document.documentElement.scrollHeight;
        var documentscrolltop = document.body.scrollTop || document.documentElement.scrollTop;
        if ((window.innerHeight + documentscrolltop) >= document.body.scrollHeight) {
            searchResult();
        }

        if (documentscrolltop >= window.innerHeight) {
            document.getElementById("top_back").style.display = "block";
        } else {
            document.getElementById("top_back").style.display = "none";
        }
    });

    function searchResult() {
        num++;
        var data_send = KPC.SearchResult.RouteInfo + "&pagenum=" + num;

        if (IsSendRequest) {
            document.getElementById("shade").style.display = "block";
            var xmlhttp;
            location.hash = num;
            if (window.XMLHttpRequest) {
                xmlhttp = new XMLHttpRequest();
                xmlhttp.onreadystatechange = function () {
                    if (xmlhttp.readyState === 4 && xmlhttp.status === 200) {
                        var data = JSON.parse(xmlhttp.responseText);

                        if (data.flag) {
                            var template_script = document.getElementById("routelist_template").innerHTML;
                            var template_HTML = _.template(template_script, { datas: data.dataList });

                            document.getElementById("searchresult").innerHTML += template_HTML;
                        } else {
                            IsSendRequest = false;
                        }

                        // hide
                        document.getElementById("shade").style.display = "none";
                        if (flag && hashNum > num) {
                            searchResult();
                        }
                    }
                };

                var url = "/api/searchresult";

                xmlhttp.open("POST", url, true);
                xmlhttp.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");// important
                xmlhttp.send(encodeURI(data_send));
            }
        }
    }

})();