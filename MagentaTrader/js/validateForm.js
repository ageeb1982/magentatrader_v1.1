function FormValidator() {
    console.log("hello");
    $("#eventForm").validate({
        debug: true,
        rules: {
            EventEdit_EventDescription : "required"
        },
        messages: {
            EventEdit_EventDescription : "Please enter a valid Description"
        },
        submitHandler: function (form) {
            form.submit();
        }
    })
}

function FormValidate() {
    var validator = $('form').validate({
        rules: {
            "EventEdit_EventDescription": "required"
        }
    });
    var x = validator.form();
    console.log(x);
    return x;
}