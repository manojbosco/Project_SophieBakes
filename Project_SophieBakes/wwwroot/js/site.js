$(document).ready(function () {
    console.log("jQuery Loaded:", typeof $ !== "undefined");
    console.log("jQuery Validate Loaded:", typeof $.fn.validate);
    console.log("Login Button Exists:", $("#loginButton").length);

    // ✅ Set default values dynamically (for debugging)
    $("input[name='Email']").attr("value", "test@example.com");
    $("input[name='Password']").attr("value", "password123");

    $("#loginButton").on("click", function (event) {
        event.preventDefault(); // Prevents form submission
        console.log("Button Clicked");

        // ✅ Debugging: Check if values are set
        console.log("Email:", $("input[name='Email']").val());
        console.log("Password:", $("input[name='Password']").val());

        // ✅ Submit form if validation passes
        if ($("form").valid()) {
            $("form").submit();
        } else {
            console.log("Form validation failed.");
        }
    });
});
