var logoUrl = document.getElementById("logo");
if (logoUrl) {
    var url = location.href;
    var x = url.lastIndexOf("/swagger");

    if (x == 0) {
        x = url.length;
    }

    logoUrl.href = url.substring(0, x);
}

var imgLst = document.getElementsByClassName("logo__img");
if (imgLst && imgLst.length>0) {
    var img = imgLst[0];
    img.src = logoUrl.href+"/content/nexus_logo.png";
}

var tltLst = document.getElementsByClassName("logo__title");
if (tltLst && tltLst.length > 0) {
    var title = tltLst[0];
    title.innerText = "ECUAFACT";
}

var footLst = document.getElementsByClassName("footer");
if (footLst && footLst.length > 0) {
    var footext = footLst[0];
    footext.innerHTML = footext.innerHTML.replace(",", logoUrl);
}
 