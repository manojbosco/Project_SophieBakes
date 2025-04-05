document.addEventListener("DOMContentLoaded", function () {
    // Add-to-cart form submission handler
    document.querySelectorAll(".add-to-cart-form").forEach(form => {
        form.addEventListener("submit", function (event) {
            event.preventDefault(); // Prevent default form submission

            let formData = new FormData(this); // Collect form data

            fetch(this.action, { // Send AJAX request
                method: "POST",
                body: formData,
                headers: {
                    "X-Requested-With": "XMLHttpRequest" // Ensure it's an AJAX request
                }
            })
                .then(response => response.json()) // Convert response to JSON
                .then(data => {
                    if (data.success) {
                        alert("Product added to cart!");
                        updateCartCount(data.cartCount);
                    } else {
                        alert("Error adding product to cart.");
                    }
                })
                .catch(error => console.error("Error:", error));
        });
    });

    // ✅ Fetch & Update Cart Count on Page Load
    fetch("/Cart/GetCartCount")
        .then(response => response.json())
        .then(data => updateCartCount(data.cartItemCount))
        .catch(error => console.error("Error fetching cart count:", error));
});

// ✅ Function to Update Cart Count Badge
function updateCartCount(count) {
    $('#cart-count').text(count);
}

$(document).ready(function () {
    $(document).on('change', '.quantity', function () {
        var productId = $(this).data('product-id');
        var newQuantity = $(this).val();

        console.log("📤 Sending AJAX request");
        console.log("🔍 Product ID:", productId, "New Quantity:", newQuantity);

        $.ajax({
            url: '/Cart/UpdateQuantity',
            method: 'POST',
            data: { productId: productId, quantity: newQuantity },
            success: function (response) {
                console.log("✅ Response:", response);
                if (response.success) {
                    $('#cart-container').html(response.cartHtml);
                    $('#total').text('Total: $' + response.total.toFixed(2));
                } else {
                    alert(response.message);
                }
            },
            error: function (xhr) {
                console.error("❌ Error:", xhr.responseText);
                alert("Something went wrong. Please try again.");
            }
        });
    });
});




// Handle remove item click
    $(document).ready(function () {
        $(document).on('click', '.remove-item', function () {
            var productId = $(this).data('product-id');

            $.ajax({
                url: '/Cart/RemoveItem',
                method: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({productId: productId}),
                success: function (response) {
                    if (response.success) {
                        // Update the cart UI
                        $('#cart-container').html(response.cartHtml);
                        // Update the total
                        $('#total').text('Total: ₹' + response.total.toFixed(2));
                        // Update the cart count
                        updateCartCount(response.cartCount);
                    } else {
                        alert('Failed to remove item.');
                    }
                },
                error: function (xhr) {
                    console.error("Error:", xhr.responseText);
                    alert("Something went wrong. Please try again.");
                }
            });
        });
    });

