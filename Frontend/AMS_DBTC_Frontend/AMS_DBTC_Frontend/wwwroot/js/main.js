$(document).ready(function () {

    $("#btnLogin").click(function (e) {
        e.preventDefault(); // 🔥 prevent form reload

        const email = $("#email").val();

        if (!email) {
            alert("Email is required");
            return;
        }

        $.post("/Auth/SetSession", {
            email: email,
            role: "teacher"
        })
            .done(function (res) {
                console.log("Login success:", res);

                // 🔥 redirect safely
                window.location.href = "/Student/Index";
            })
            .fail(function (err) {
                console.error("Login failed:", err);
                alert("Login failed");
            });

    });

});