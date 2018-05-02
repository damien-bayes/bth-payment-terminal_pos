const Cookie = {
   create: function (name, value, minutes) {
     let expires = '';

     if (minutes) {
       let date = new Date();
       date.setTime(date.getTime() + (minutes * 60 * 1000));
       expires = "; expires=" + date.toGMTString();
     }

     document.cookie = name + "=" + value + expires + "; path=/";
   },
   read: function (name) {
        let nameEQ = name + "=";
        let ca = document.cookie.split(";");
        for (let i = 0; i < ca.length; i++) {
            let c = ca[i];
            while (c.charAt(0) == " ") c = c.substring(1, c.length);
            if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
        }
        return null;
    },
    erase: function (name) {
        Cookie.create(name, "", -1);
    }
};
