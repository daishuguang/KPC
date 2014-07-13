
//
$(document).ready(function () {
    $("#btnDel").on("click", function () {
        $("#menu2").toggleClass("menu2");
        deleteRoute(this); return false;
    });

    $("#menu, #menu2").on("click", function () {
        $("#menu2").toggleClass("menu2");
    });
});

function deleteRoute(owner) {

    $(owner).attr("disabled", "disabled");

    if (!window.confirm("您确认要删除此路线吗？")) {
        return false;
    }

    sendPostRequest(
		'/api/DeleteRoute',
		'{ "userID": "' + _userID + '", "routeID": "' + _routeID + '" }',
		function (datas) {
		    $(owner).removeAttr("disabled");

		    if (datas.Status == 0) {
		        window.location.href = "/route/list";
		        //window.location.reload();
		    }
		    else {
		        alert(":( 操作失败！您可以再试试，有任何问题要告诉小拼哟！");
		    }
		}
	);
}

window.addEventListener("scroll", function () {
    var documentscrolltop = document.body.scrollTop || document.documentElement.scrollTop;

    if (documentscrolltop >= window.innerHeight) {
        document.getElementById("top_back").style.display = "block";
    } else {
        document.getElementById("top_back").style.display = "none";
    }
});