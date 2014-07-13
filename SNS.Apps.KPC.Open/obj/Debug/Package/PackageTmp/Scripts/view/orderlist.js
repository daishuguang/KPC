(function () {
    var IsSendRequest = true, num = -1;

    window.addEventListener("scroll", function () {
        // 加载更多
        var documentscrolltop = document.body.scrollTop || document.documentElement.scrollTop;
        if ((window.innerHeight + documentscrolltop) >= document.body.scrollHeight - 3) {
            moreOrderList();
        }

        // back to top
        if (documentscrolltop > window.innerHeight) {
            document.getElementById("top_back").style.display = "block";
        } else {
            document.getElementById("top_back").style.display = "none";
        }
    });

    document.addEventListener("DOMContentLoaded", DOMContentLoaded, false);

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

    function DOMContentLoaded() {
        moreOrderList();
    }

    function moreOrderList() {
        num++;

        var data_send = {
            ordertype: KPC.OrderList.OrderType,
            pagenum: num,
        };

        if (IsSendRequest) {
            document.getElementById("shade").style.display = "block";
            var xmlhttp;
            if (window.XMLHttpRequest) {
                xmlhttp = new XMLHttpRequest();
                xmlhttp.onreadystatechange = function () {
                    if (xmlhttp.readyState === 4 && xmlhttp.status === 200) {
                        var data = JSON.parse(xmlhttp.responseText);

                        if (data.flag) {
                            var template_script = document.getElementById("orderlist_template").innerHTML;
                            var template_HTML = _.template(template_script, { datas: data.dataList });

                            document.getElementById("orderlist").innerHTML += template_HTML;
                        } else {
                            IsSendRequest = false;
                        }

                        // hide
                        document.getElementById("shade").style.display = "none";
                    }
                };

                var url = "/api/loaduserorders?ordertype=" + data_send.ordertype + "&pagenum=" + data_send.pagenum;

                xmlhttp.open("GET", url, true);
                xmlhttp.send(null);
            }
        }
    }
})();