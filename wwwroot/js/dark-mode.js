document.addEventListener("DOMContentLoaded", function () {
    console.log("Dark mode script loaded"); // Kiểm tra xem script có được tải không
    const toggleButton = document.getElementById("toggle-dark-mode");
    const body = document.body;

    if (!toggleButton) {
        console.error("Toggle button not found");
        return;
    }

    // Kiểm tra chế độ đã lưu trong localStorage
    if (localStorage.getItem("theme") === "dark") {
        body.classList.add("dark-mode");
        toggleButton.textContent = "☀️ Light Mode";
    }

    // Xử lý sự kiện khi nhấn nút
    toggleButton.addEventListener("click", function () {
        if (body.classList.contains("dark-mode")) {
            body.classList.remove("dark-mode");
            toggleButton.textContent = "🌙 Dark Mode";
            localStorage.setItem("theme", "light");
        } else {
            body.classList.add("dark-mode");
            toggleButton.textContent = "☀️ Light Mode";
            localStorage.setItem("theme", "dark");
        }
    });
});