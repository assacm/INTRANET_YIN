!(function ($) {
    "use strict";

    const btnsCollapse = document.querySelectorAll('[data-bs-toggle="collapse"]');

    btnsCollapse.forEach((btn) => {

        const collapse = document.querySelector(btn.getAttribute('href'));
        btn.addEventListener('click', function () {

            const expanded = this.getAttribute('aria-expanded') === 'true' || false;
            this.setAttribute('aria-expanded', !expanded);

            collapse.classList.toggle('show');

            collapse.style.transition = "height 1s ease-in-out";
        });
    });




    const navItems = document.querySelectorAll('.nav-item');
    const navTabs = document.querySelector('#nav-tab');
    const tabContent = document.querySelector('#nav-tabContent');

    // Agrega un evento de clic a cada elemento de pestaña
    navItems.forEach(item => {
        item.addEventListener('click', () => {
            // Desactiva la pestaña activa anterior
            navTabs.querySelector('.active').classList.remove('active');
            item.classList.add('active');

            // Oculta el contenido activo anterior
            tabContent.querySelector('.active').classList.remove('show', 'active');

            // Obtiene el contenido de la pestaña actual
            const target = item.dataset.bsTarget;
            const tabPane = tabContent.querySelector(target);

            // Muestra el contenido de la pestaña actual
            tabPane.classList.add('show', 'active');
        });
    });


    if (window.innerWidth <= 992) {
        $(".btn-sidebar").css("display", "none");
        $("#menu").removeClass("ocultar");
    }

    // Smooth scroll for the navigation menu and links with .scrollto classes
    var scrolltoOffset = $('#header').outerHeight() - 21;
    if (window.matchMedia("(max-width: 991px)").matches) {
        scrolltoOffset += 20;
    }
    $(document).on('click', '.nav-menu a, .mobile-nav a, .scrollto', function (e) {
        if (location.pathname.replace(/^\//, '') == this.pathname.replace(/^\//, '') && location.hostname == this.hostname) {
            var target = $(this.hash);
            if (target.length) {
                e.preventDefault();

                var scrollto = target.offset().top - scrolltoOffset;

                if ($(this).attr("href") == '#header') {
                    scrollto = 0;
                }

                $('html, body').animate({
                    scrollTop: scrollto
                }, 1500, 'easeInOutExpo');

                if ($(this).parents('.nav-menu, .mobile-nav').length) {
                    $('.nav-menu .active, .mobile-nav .active').removeClass('active');
                    $(this).closest('li').addClass('active');
                }

                if ($('body').hasClass('mobile-nav-active')) {
                    $('body').removeClass('mobile-nav-active');
                    $('.mobile-nav-toggle i').toggleClass('icofont-navigation-menu icofont-close');
                    $('.mobile-nav-overly').fadeOut();
                }
                return false;
            }
        }
    });

    // Activate smooth scroll on page load with hash links in the url
    $(document).ready(function () {
        if (window.location.hash) {
            var initial_nav = window.location.hash;
            if ($(initial_nav).length) {
                var scrollto = $(initial_nav).offset().top - scrolltoOffset;
                $('html, body').animate({
                    scrollTop: scrollto
                }, 1500, 'easeInOutExpo');
            }
        }
    });

    var fullHeight = function () {

        $('.js-fullheight').css('height', $(window).height());
        $(window).resize(function () {
            $('.js-fullheight').css('height', $(window).height());
        });

    };
    fullHeight();

    $('#sidebarCollapse').on('click', function () {
        $('#sidebar').toggleClass('active');
    });


    // Mobile Navigation
    if ($('.nav-menu').length) {
        var $mobile_nav = $('.nav-menu').clone().prop({
            class: 'mobile-nav d-lg-none'
        });
        $('body').append($mobile_nav);
        $('body').prepend('<button type="button" class="mobile-nav-toggle d-lg-none"><i class="icofont-navigation-menu"></i></button>');
        $('body').append('<div class="mobile-nav-overly"></div>');

        $(document).on('click', '.mobile-nav-toggle', function (e) {
            $('body').toggleClass('mobile-nav-active');
            $('.mobile-nav-toggle i').toggleClass('icofont-navigation-menu icofont-close');
            $('.mobile-nav-overly').toggle();
        });

        $(document).on('click', '.mobile-nav .drop-down > a', function (e) {
            e.preventDefault();
            $(this).next().slideToggle(300);
            $(this).parent().toggleClass('active');
        });

        $(document).click(function (e) {
            var container = $(".mobile-nav, .mobile-nav-toggle");
            if (!container.is(e.target) && container.has(e.target).length === 0) {
                if ($('body').hasClass('mobile-nav-active')) {
                    $('body').removeClass('mobile-nav-active');
                    $('.mobile-nav-toggle i').toggleClass('icofont-navigation-menu icofont-close');
                    $('.mobile-nav-overly').fadeOut();
                }
            }
        });
    } else if ($(".mobile-nav, .mobile-nav-toggle").length) {
        $(".mobile-nav, .mobile-nav-toggle").hide();
    }

    // Toggle .header-scrolled class to #header when page is scrolled
    $(window).scroll(function () {
        if ($(this).scrollTop() > 100) {
            $('#header').addClass('header-scrolled');
            $('#topbar').addClass('topbar-scrolled');
        } else {
            $('#header').removeClass('header-scrolled');
            $('#topbar').removeClass('topbar-scrolled');
        }
    });

    if ($(window).scrollTop() > 100) {
        $('#header').addClass('header-scrolled');
        $('#topbar').addClass('topbar-scrolled');
    }


})(jQuery);