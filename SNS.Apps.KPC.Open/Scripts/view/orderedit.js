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
    document.getElementById("txt_charge").addEventListener("keyup", function () {
        this.value = this.value.replace(/^0|\D/g, '');
    });

    document.getElementById("button_agree").addEventListener("click", function () {
        document.getElementById("shade_bottom").style.display = "none";
        document.getElementById("agreement").style.display = "none";
    });

    document.getElementById("backbtn").addEventListener("click", function () {
        history.back();
    });

})();

function show() {
    document.getElementById("shade_bottom").style.display = "block";
    document.getElementById("agreement").style.display = "block";
}
window.addEventListener("scroll", function () {
    var documentscrolltop = document.body.scrollTop || document.documentElement.scrollTop;
    document.getElementById("shade_bottom").style.height = window.innerHeight + documentscrolltop + "px";
});