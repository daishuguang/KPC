(function () {
    document.getElementById("button_agree").addEventListener("click", function () {
        document.getElementById("shade_bottom").style.display = "none";
        document.getElementById("agreement").style.display = "none";
    });

    document.getElementById("backbtn").addEventListener("click", function () {
        if (refernull) {
            if (message) {
                WeixinJSBridge.call("closeWindow");
            } else {
				WeixinJSBridge.call("closeWindow");
                window.location.href = "/order/list";
            }
        } else {
            history.back();
        }
    });

    if (document.getElementById("menu") !== "undefined" && document.getElementById("menu") !== null) {
        document.getElementById("menu").addEventListener("click", toggleClass);

        document.getElementById("menu2").addEventListener("click", hideMenu);

        document.body.addEventListener("click", hideMenu);
    }
    function hideMenu(e) {
        if (e.target !== document.getElementById("menu")) {
            document.getElementById("menu2").className = "menu2";
        }
    }
    if (document.getElementById("btn_pay") !== "undefined" && document.getElementById("btn_pay") !== null) {
        document.getElementById("btn_pay").addEventListener("click", function () {
            document.forms[0].action = "/payment/pay";
            document.forms[0].method = "POST";
            document.forms[0].submit();
        });

        document.getElementById("pay_ways1").addEventListener("click", function () {
            if (this.checked === "checked" || this.checked === true) {
                document.getElementById("cash1").disabled = false;
                document.getElementById("cash1").checked = true;
                document.getElementById("way1").style.display = "";
                document.getElementById("way2").style.display = "none";
                document.getElementById("way3").style.display = "none";
                if (MianYi) {
                    document.getElementById("btn_pay").className = "btn_disable";
                    document.getElementById("btn_pay").disabled = true;
                }
                if (XianXia && !MianYi) {
                    document.getElementById("btn_pay").className = "";
                    document.getElementById("btn_pay").disabled = false;
                }
            }
        });
        document.getElementById("pay_ways2").addEventListener("click", function () {
            if (this.checked === "checked" || this.checked === true) {
                document.getElementById("cash1").disabled = false;
                document.getElementById("cash1").checked = true;
                document.getElementById("way1").style.display = "none";
                document.getElementById("way2").style.display = "";
                document.getElementById("way3").style.display = "none";
                document.getElementById("btn_pay").disabled = false;
                document.getElementById("btn_pay").className = "";
            }
        });
        document.getElementById("pay_ways3").addEventListener("click", function () {
            if (this.checked === "checked" || this.checked === true) {
                document.getElementById("cash1").checked = false;
                document.getElementById("cash1").disabled = true;
                document.getElementById("way1").style.display = "none";
                document.getElementById("way2").style.display = "none";
                document.getElementById("way3").style.display = "";
                if (XianXia) {
                    document.getElementById("btn_pay").disabled = true;
                    document.getElementById("btn_pay").className = "btn_disable";
                }
            }
        });

        document.getElementById("cash1").addEventListener("click", function () {
            if (document.getElementById("pay_ways3").checked === true) {
                this.checked === false;
                this.disabled === true;
            }
        });
    }
})();

function show() {
    document.getElementById("shade_bottom").style.display = "block";
    document.getElementById("agreement").style.display = "block";
}

function funCancel(e) {
    if (!confirm("您确定要取消拼单吗")) {
        return false;
    } else {
        showWait();
        document.forms[0].action = "/order/cancel";
        document.forms[0].submit();
    }
}

function funConfirm_Passenger(e) {
    e.preventDefault();

    if (!confirm("请确认您已上车，并顺利达到目的地！")) {
        return false;
    } else {
        document.forms[0].action = "/order/confirm";
        document.forms[0].submit();
    }
}

function funConfirm_Driver(e) {
    e.preventDefault();

    if (!confirm("请确认您已顺利接车，并已收到拼车余款！")) {
        return false;
    } else {
        document.forms[0].action = "/order/confirm";
        document.forms[0].submit();
    }
}

function toggleClass() {
    if (document.getElementById("menu2").className === "menu2") {
        document.getElementById("menu2").className = "";
    } else {
        document.getElementById("menu2").className = "menu2";
    }
}