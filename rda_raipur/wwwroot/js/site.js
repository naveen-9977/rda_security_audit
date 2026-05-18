document.addEventListener("DOMContentLoaded", function () {
    // Check karein ki kya user ne pehle popup dekh liya hai
    if (!localStorage.getItem("hasSeenPopup")) {

        // Modal element ko select karein
        var popupElement = document.getElementById('welcomePopupModal');

        if (popupElement) {
            // Bootstrap modal ko initialize karein
            var myModal = new bootstrap.Modal(popupElement);

            // Modal ko show karein
            myModal.show();

            // localStorage mein flag set kar dein taaki aage ye dubara na dikhe
            localStorage.setItem("hasSeenPopup", "true");
        }
    }
});