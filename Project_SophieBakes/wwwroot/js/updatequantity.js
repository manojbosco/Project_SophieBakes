<script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
<script>
    $(document).ready(function () {
    $(".update-btn").click(function (e) {
        e.preventDefault(); // Prevent form submission (important!)

        var productId = $(this).data("product-id");
        var quantity = $(this).closest("tr").find(".quantity-input").val();

        if (!productId || !quantity) {
            alert("Error: Missing product ID or quantity.");
            return;
        }

        $.ajax({
            url: "/Cart/UpdateQuantity",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({ productId: productId, quantity: parseInt(quantity) }),
            success: function (response) {
                if (response.success) {
                    location.reload(); // Reload cart on success
                } else {
                    alert(response.message); // Show error message
                }
            },
            error: function (xhr, status, error) {
                console.error("AJAX Error:", error);
                alert("Something went wrong. Please try again.");
            }
        });
    })
}); // ✅ Properly closed $(document).ready()
</script> 

