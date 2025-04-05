function filterProducts(category) {
    var allProducts = document.querySelectorAll(".product-box");

    allProducts.forEach(function (product) {
        if (category === "All") {
            product.style.display = "block";
        } else if (product.classList.contains(category.toLowerCase())) {
            product.style.display = "block";
        } else {
            product.style.display = "none";
        }
    });
}
