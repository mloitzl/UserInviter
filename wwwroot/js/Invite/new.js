var appStorePrefix = "com.loitl.userinviter-";

function saveToLocalStorage(key, value) {
    console.log(appStorePrefix + key + " => " + value);
    localStorage.setItem(appStorePrefix + key, value);
}

function getFromLocalStorage(key) {
    return localStorage.getItem(appStorePrefix + key);
}


$(function () {
    $("form input#RedirectUri").blur(function (e, h) {
        saveToLocalStorage(e.target.nodeName + "#" + e.target.id, e.target.value);
    });

    $("form input#RedirectUri").focus(function (e, h) {

        if(!e.target.value || !e.target.value.trim()){
            $(this).val(getFromLocalStorage(e.target.nodeName + "#" + e.target.id));
            $(this).select();
        }
    });
});