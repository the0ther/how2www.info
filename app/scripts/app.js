/*global define */
define(['Mergesort'], function (Mergesort) {
    'use strict';

    console.log(arguments);
    console.log(Mergesort);
    var sort = new Mergesort();
    sort.sort([8, -4, 9, 1, 2]);
});