(function () {
    var phoneRegExp = new RegExp(/^[1][358][0-9]{9}$/),
        count = 30,
        clickCount = 0,
        channelflag = 1,
        seed = null;

    $(document).ready(function () {
        $("#btn_submit").on("click", formSubmit);
        $("input").on("keydown", preventSubmit);
        //$("input:radio").on("change", select_change);
        $("#btnCode").on("click", setCode);
        document.getElementById("backbtn").addEventListener("click", function () {
            history.back();
        });
    });

    /*
    * el: submit
    * event: tap
    * func: submit
    */
    function formSubmit(e) {
        var phoneNum = $.trim($("#txt_mobilenum").val()),
            verifycode = $.trim($("#txt_verifycode").val()),
            nickname = $.trim($("#txt_nickname").val()),
            //select_userRole = $("input:radio:checked").val(),
            //txt_licencePlateNumber = $.trim($("#txt_licencePlateNumber").val()),
            flag = true;

        /* validate nickname */
        if (nickname === "") {
            alert("亲，昵称忘填了吧");
            flag = false;
            return;
        }
        /* validate phoneNum */
        switch (phoneNum) {
            case "":
                alert("亲，填写手机号方便拼友联系");
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

        if (verifycode === "") {
            alert("亲，验证码忘填了吧");
            return;
        }

        if (KPC.Register.checkPassword) {
            var password1 = $.trim($("#txt_password1").val()),
                password2 = $.trim($("#txt_password2").val());
            if (password1.length < 6 || password2.length < 6) {
                alert("密码长度不能少于6位");
                flag = false;
                return;
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
        }
        if (document.getElementById("txt_refcode") !== null) {
            if (document.getElementById("txt_refcode").value !== "" && document.getElementById("txt_refcode").value.length !== 6) {
                alert("亲，推荐码是6位");
                return;
            }
        }
        /*  submit  */
        if (flag) {
            $("#btn_submit").prop("disabled", true);
            showWait();
            $("form").submit();
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

    /*
    * el: select_userrole
    * event: change
    * func: select_change
    */
    function select_change() {
        var userrole = $("input:radio:checked").attr("id");
        var $li_licencePlateNumber = $("#li_licencePlateNumber");

        if (userrole === "radiodriver") {
            $li_licencePlateNumber.css("display", "block");
        } else {
            $li_licencePlateNumber.css("display", "none")
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
        var phone = $.trim($("#txt_mobilenum").val());
        if (!phoneRegExp.test(phone)) {
            alert("亲，您的手机号填错了吧");
            return;
        }
        count = 30;
        $(e.currentTarget).off("click");
        getNumber();

        $.ajax({
            url: "/api/verifyphonenumber",
            type: "POST",
            data: JSON.stringify({ phonenum: phone, channel: channelflag }),
            contentType: "application/json; charset=utf-8",
            dataType: "json"
        }).done(function (data, textStatus, jqXHR) {
            if (!data.flag) {
                clearTimeout(seed);
                $("#btnCode").text("重新获取");
                document.getElementById("btnCode").className = "verify_able";
                $("#btnCode").on("click", setCode);
            }
        }).fail(function (e) {
            clearTimeout(seed);
            $("#btnCode").text("重新获取");
            document.getElementById("btnCode").className = "verify_able";
            $("#btnCode").on("click", setCode);
        });
        channelflag = (channelflag === 0 ? 1 : 0);
    }

    function getNumber() {
        count--;
        $("#btnCode").text(count + "秒后可重发");
        document.getElementById("btnCode").className = "verify_disable";
        if (count > 0) {
            seed = setTimeout(getNumber, 1000);
        } else {
            $("#btnCode").text("重新获取");
            document.getElementById("btnCode").className = "verify_able";
            $("#btnCode").on("click", setCode);
        }
    }

    //$("input:radio").trigger("change");
})();
