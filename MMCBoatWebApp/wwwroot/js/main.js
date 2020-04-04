
(function ($) {
    "use strict";

    
    /*==================================================================
    [ Validate ]*/
    var input = $('.validate-input .input100, .validate-input .form-control');

    var inputPhone = $('.validate-input-phone .input100, .validate-input-phone .form-control');

    $('.validate-form').on('submit',function(){
        var check = true;

        for(var i=0; i<input.length; i++) {
            if(validate(input[i]) == false){
                showValidate(input[i]);
                check=false;
            }
        }

        for (var i = 0; i < inputPhone.length; i++) {
            if (validatePhone(inputPhone[i]) == false) {
                showValidatePhone(inputPhone[i]);
                check = false;
            }
        }

        return check;
    });


    $('.validate-form .input100, .validate-input .form-control').each(function(){
        $(this).focus(function(){
           hideValidate(this);
        });
    });

    $('.validate-form .input100, .validate-input-phone .form-control').each(function () {
        $(this).focus(function () {
            hideValidatePhone(this);
        });
    });

    function validate (input) {
        if($(input).attr('type') == 'email' || $(input).attr('name') == 'email') {
            if($(input).val().trim().match(/^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{1,5}|[0-9]{1,3})(\]?)$/) == null) {
                return false;
            }
        }
        else {
            if($(input).val().trim() == ''){
                return false;
            }
        }
    }


    function validatePhone(inputPhone) {

        if ($(inputPhone).attr('type') == 'tel' || $(inputPhone).attr('name') == 'tel') {
            if ($(inputPhone).val().trim() == '') {
                return true;
            }
            else if ($(inputPhone).val().trim().match(/^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$/) == null) {
                return false;
            }
        }
    }

    function showValidate(input) {
        var thisAlert = $(input).parent();

        $(thisAlert).addClass('alert-validate');
    }

    function hideValidate(input) {
        var thisAlert = $(input).parent();

        $(thisAlert).removeClass('alert-validate');
    }

    function showValidatePhone(input) {
        var thisAlert = $(input).parent();

        $(thisAlert).addClass('alert-validate');
    }

    function hideValidatePhone(input) {
        var thisAlert = $(input).parent();

        $(thisAlert).removeClass('alert-validate');
    }

})(jQuery);