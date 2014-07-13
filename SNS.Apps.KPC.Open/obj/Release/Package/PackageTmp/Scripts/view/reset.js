(function () {
    var phoneRegExp = new RegExp(/^[1][358][0-9]{9}$/),
        count = 30,
        clickCount = 0,
        flag = 1,
        seed = null;

    document.getElementById("btn_submit").addEventListener("click", formSubmit);
    document.getElementById("keydown", preventSubmit);
    document.getElementById("txt_verifycode").addEventListener("click", setCode);
    document.getElementById("backbtn").addEventListener("click", function () {
        history.back();
    });
    /*
    * el: submit
    * event: tap
    * func: submit
    */
    function formSubmit(e) {
        var phoneNum = document.getElementById("txt_mobile").value.replace(/\s/, ""),
            password1 = document.getElementById("txt_password1").value,
            password2 = document.getElementById("txt_password2").value,
        flag = true;

        /* validate phoneNum */
        switch (phoneNum) {
            case "":
                alert("亲，手机号不能为空");
                flag = false;
                return;
            default:
                if (!phoneRegExp.test(phoneNum)) {
                    alert("亲，您的手机号填错了吧");
                    flag = false;
                    return;
                }
                break;
        }

        if (password1 === "" || password2 === "") {
            alert("密码不能为空");
            flag = false;
            return;
        }
        if (password1 !== password2) {
            alert("亲，密码不一致");
            flag = false;
            return;
        }
        /*  submit  */
        if (flag) {
            document.getElementById("btn_submit").disabled = "disabled";
            // submit
            showWait();
            document.forms[0].submit();
        }
    }

    /*
    * el: btnCode
    * event: click
    * func: set code
    */
    function setCode(e) {
        if (clickCount >= 10) {
            alert("请回复微信，联系我们的客服专员");
            return;
        } else {
            clickCount++;
        }
        var phone = document.getElementById("txt_mobile").value.replace(/\s/, "");
        if (!phoneRegExp.test(phone)) {
            alert("亲，您的手机号填错了吧");
            return;
        }
        count = 30;
        e.currentTarget.removeEventListener("click");
        getNumber();

        var xmlhttp;
        if (window.XMLHttpRequest) {
            var data_send = "phonenum=" + phone + "&channel=" + flag;
            xmlhttp = new XMLHttpRequest();
            xmlhttp.onreadystatechange = function () {
                if (xmlhttp.status === 200 && xmlhttp.readyState === 4) {
                    var data = JSON.parse(xmlhttp.responseText);

                    if (!data.flag) {
                        clearTimeout(seed);
                        document.getElementById("txt_verifycode").innerText = "重新获取";
                        document.getElementById("txt_verifycode").className = "verify_able";
                        document.getElementById("txt_verifycode").addEventListener("click", setCode);
                    }
                }
                if (xmlhttp.readState === 4 && xmlhttp.status == 404) {
                    clearTimeout(seed);
                    document.getElementById("txt_verifycode").innerText = "重新获取";
                    document.getElementById("txt_verifycode").className = "verify_able";
                    document.getElementById("txt_verifycode").addEventListener("click", setCode);
                }
            };

            var url = "/api/verifyphonenumber";

            xmlhttp.open("POST", url, true);
            xmlhttp.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            xmlhttp.send(encodeURI(data_send));
        }
        flag = (flag === 0 ? 1 : 0);
    }

    function getNumber() {
        count--;
        document.getElementById("txt_verifycode").innerText = count + "秒后可重发";
        document.getElementById("txt_verifycode").className = "verify_disable";
        if (count > 0) {
            seed = setTimeout(getNumber, 1000);
        } else {
            document.getElementById("txt_verifycode").innerText = "重新获取";
            document.getElementById("txt_verifycode").className = "verify_able";
            document.getElementById("txt_verifycode").addEventListener("click", setCode);
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