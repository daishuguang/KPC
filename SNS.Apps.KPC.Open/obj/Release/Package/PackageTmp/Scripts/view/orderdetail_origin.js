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

    
    $("#menu, #menu2").on("click", function () {
        $("#menu2").toggleClass("menu2");
    });

    $("#txt_charge").on("keyup", function () {
        this.value = this.value.replace(/^0|\D/g, '');
    });

    // Bind Events
    // Cancel
    $("#btn_cancel_driver").on("tap", function () { onCancelOrder(this); });
    $("#btn_cancel_passenger").on("tap", function () { onCancelOrder(this); });

    // Confirm
    $("#btn_confirm_ok_driver").on("tap", function () { onConfirmOrder(this, true); });
    $("#btn_confirm_cancel_driver").on("tap", function () { onConfirmOrder(this, false); });
    $("#btn_confirm_ok_passenger").on("tap", function () { onConfirmOrder(this, true); });
    $("#btn_confirm_cancel_passenger").on("tap", function () { onConfirmOrder(this, false); });

    // Submit

});



function onCancelOrder(owner) {
    $(owner).attr("disabled", "disabled");

    sendPostRequest(
		"/api/cancelorder",
		"{\"userID\": \"" + _userID + "\", \"folio\": \"" + _fo_folio + "\"}",
		function (datas) {
		    if (datas.Status == 0) {
		        alert("已成功取消此订单!");
		    }
		    else {
		        alert("哎呀，小拼不给力，没能完成您的指令，再次点击按钮试试看哟!");
		    }

		    refreshPage();
		}
	);
}

function onConfirmOrder(owner, isOK) {
    $(owner).attr("disabled", "disabled");

    sendPostRequest(
		"/api/confirmorder",
		"",
		function (datas) {
		    if (datas.Status == 0) {
		        alert("已成功取消此订单!");
		    }
		    else {
		        alert("哎呀，小拼不给力，没能完成您的指令，再次点击按钮试试看哟!");
		    }

		    refreshPage();
		}
	);
}

function onEditOrder(owner) {
    $(owner).attr("disabled", "disabled");

    sendPostRequest(
		"/api/confirmorder",
		"",
		function (datas) {
		    if (datas.Status == 0) {
		        alert("已成功更新了您的拼单!");
		    }
		    else {
		        alert("哎呀，小拼不给力，没能完成您的指令，再次点击按钮试试看哟!");
		    }

		    refreshPage();
		}
	);
}