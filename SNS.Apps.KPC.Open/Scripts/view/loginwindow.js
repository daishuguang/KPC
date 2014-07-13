(function () {
    var phoneRegExp = new RegExp(/^[1][358][0-9]{9}$/),
        count = 60,
        clickCount = 0,
        flag = 1,
        seed = null;

		document.getElementById("backbtn").addEventListener("click", function () {
        history.back();
    });
    document.getElementById("btn_submit").addEventListener("click", formSubmit);
    document.getElementById("keydown", preventSubmit);
    /*
    * el: submit
    * event: tap
    * func: submit
    */
    function formSubmit(e) {
    	var phoneNum = document.getElementById("txt_mobile").value.replace(/\s/, "");
        var flag = true;

        /* validate phoneNum */
        switch (phoneNum) {
            case "":
                alert("亲，手机号不能为空！");
                flag = false;
                return;
            default:
                if (!phoneRegExp.test(phoneNum)) {
                    alert("亲，您的手机号填错了吧！");
                    flag = false;
                    return;
                }
                break;
        }
		/* End */

    	/* Validate Password */
        if (flag) {
        	var password = document.getElementById("txt_password").value.replace(/\s/, "");

        	if (password == "") {
        		alert("亲，密码不能为空！");
        		flag = false;
        	}
        }
		/* End */

        /*  submit  */
        if (flag) {
            document.getElementById("btn_submit").disabled = "disabled";
            // submit
            showWait();
            document.forms[0].submit();
        }
    }

    /*
    * el: input
    * event: keydown
    * func: Prevent enter key submit
    */
    function preventSubmit() {
        if (window.attachEvent) {
            document.forms[0].attachEvent("onkeydown", function (e) {
                if (e.keyCode === 13) {
                    e.returnValue = true;
                    return false;
                }
            });
        } else {
            document.forms[0].addEventListener("keydown", function (e) {
                if (e.which === 13) {
                    e.preventDefault();
                    return false;
                }
            });
        }
    }
})();