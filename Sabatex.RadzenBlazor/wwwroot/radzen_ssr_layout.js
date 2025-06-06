(function () {
    document.querySelector('.rz-sidebar-toggle').addEventListener("click", function () {
        let sidebar = document.querySelector('.rz-sidebar');
        let isButtonCollapse = sidebar.classList.contains("side-bar-collapse");
        //console.log("isButtonCollapse = " + isButtonCollapse);
        let isCollapsed = sidebar.clientWidth == 0;
        //console.log("isCollapsed = " + isCollapsed);

        if (isButtonCollapse) {
            sidebar.classList.remove("side-bar-collapse");
            if (sidebar.clientWidth == 0) {
                sidebar.classList.remove("rz-sidebar-responsive");
            }
            //console.log("SideBar Show");
        } else {
            if (isCollapsed && sidebar.classList.contains("rz-sidebar-responsive")) {
                sidebar.classList.remove("rz-sidebar-responsive");
            }
            else {
                sidebar.classList.add("side-bar-collapse");
                if (!sidebar.classList.contains("rz-sidebar-responsive")) {
                    sidebar.classList.add("rz-sidebar-responsive");
                }
            }
            //console.log("SideBar Close");
        }

    })

})();
