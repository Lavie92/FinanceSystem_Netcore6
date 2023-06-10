$(document).ready(function () {
    var imageBase64 = '@(TempData["Image"] as string)';
    var backgroundImage = imageBase64 ? `url(data:image/png;base64,${imageBase64})` : '';

    $('#imagePreview').css('background-image', backgroundImage);

    if (backgroundImage !== '') {
        $('#imagePreview').hide().fadeIn(650);
    }

    $("#imageUpload").change(function () {
        readURL(this);
    });

    function readURL(input) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();
            reader.onload = function (e) {
                $('#imagePreview').css('background-image', 'url(' + e.target.result + ')');
                $('#imagePreview').hide().fadeIn(650);
            }
            reader.readAsDataURL(input.files[0]);
        }
    }
});