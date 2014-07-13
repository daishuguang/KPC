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

    $("#StartDate_Time").mobiscroll().time({
        preset: "time",
        theme: "default",
        lang: "zh",
        display: "bottom",
        mode: "scroller"
    });
})();

$(function () {
    $("#pay_ways1, #pay_ways2, #pay_ways3").on("click", function () {
        var checkedval = $(":checked[name='pay_ways']").val();
        if (checkedval === "0") {
            $("#cash1").prop({ "disabled": true, "checked": false });
        } else {
            $("#cash1").prop({ "disabled": false, "checked": true });
        }
        switch (checkedval) {
            case "2":
                $("#way2").css("display", "none");
                $("#way3").css("display", "none");
                $("#way1").css("display", "");
                if (KPC.MianYi) {
                    $("#btn_submit").addClass("btn_disable").prop("disabled", true);
                }
                break;
            case "1":
                $("#way1").css("display", "none");
                $("#way3").css("display", "none");
                $("#way2").css("display", "");
                if (KPC.MianYi) {
                    $("#btn_submit").removeClass("btn_disable").prop("disabled", false);
                }
                break;
            case "0":
                $("#way1").css("display", "none");
                $("#way2").css("display", "none");
                $("#way3").css("display", "");
                if (KPC.MianYi) {
                    $("#btn_submit").removeClass("btn_disable").prop("disabled", false);
                }
                break;
        }
    });

    /* 车主基本信息 */
    $("#txt_charge").on("keyup", function () {
        this.value = this.value.replace(/^0|\D/g, '');
    });
    /*    
    document.getElementById("btn_submit").addEventListener("click", function (e) {
        if (!confirm("你已和对方进行了电话确认？否则一切费用纠纷与快拼车无关")) {
            e.preventDefault();
        }
    });
    */
    document.getElementById("backbtn").addEventListener("click", function () {
        if (refernull) {
            WeixinJSBridge.call("closeWindow");
        } else
            history.back();
    });
    $("#chkChecked").on("change", function () {
        if (this.checked) {
            $("#btn_submit").removeAttr("disabled");
        }
        else {
            $("#btn_submit").attr("disabled", "disabled");
        }
    });

    document.getElementById("button_agree").addEventListener("click", function () {
        document.getElementById("shade_bottom").style.display = "none";
        document.getElementById("agreement").style.display = "none";
    });
});
function show() {
    document.getElementById("shade_bottom").style.display = "block";
    document.getElementById("agreement").style.display = "block";
}

if (document.getElementById("peoplecount") !== null) {
    document.getElementById("passengernum").addEventListener("keyup", function () {
        document.getElementById(this.id).value = document.getElementById(this.id).value.replace(/^0|\D/g, '');
    });
}

window.addEventListener("scroll", function () {
    var documentscrolltop = document.body.scrollTop || document.documentElement.scrollTop;
    document.getElementById("shade_bottom").style.height = window.innerHeight + documentscrolltop + "px";
});

$("#pay_ways1").trigger("click");