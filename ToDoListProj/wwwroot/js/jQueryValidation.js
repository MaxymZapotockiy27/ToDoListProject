<script>
    $(document).ready(function () {
        $("#loginForm").validate({
            rules: {
                "Input.Email": {
                    required: true,
                    email: true
                },
                "Input.Password": {
                    required: true,
                    minlength: 6
                }
            },
            messages: {
                "Input.Email": {
                    required: "Please enter your email",
                    email: "Please enter a valid email address"
                },
                "Input.Password": {
                    required: "Please enter your password",
                    minlength: "Your password must be at least 6 characters long"
                }
            },
            submitHandler: function (form) {
                form.submit();  
            }
        });
    });
    $(document).ready(function () {
        $("#Input_ConfirmPassword").on("input", function () {
            $(this).removeAttr("data-val-ignore");
        });
});
</script>
