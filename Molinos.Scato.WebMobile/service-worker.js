
'use strict';

const CACHE_NAME = 'static-cache-v5';
const DATA_CACHE_NAME = 'data-cache-v1';


const FILES_TO_CACHE = [

    './',
    './offline.html',

    './Content/images/icons/icon-144x144.png',
    './Index/Logo',
    './Content/images/icons/icon-72x72.png',
    './Content/images/icons/icon-96x96.png',
    './Content/images/icons/icon-128x128.png',
    './Content/images/icons/icon-152x152.png',
    './Content/images/icons/icon-192x192.png',
    './Content/images/icons/icon-384x384.png',
    './Content/images/icons/icon-512x512.png',
    './manifest.json',
    './Scripts/install.js',
    './service-worker.js',
    
    //'./scripts/knockout-2.2.1.js',
    //'./scripts/Chart.js',
    //'./Scripts/Chart.bundle.js',
    //'./Scripts/chartjs-plugin-annotation.min.js',
    //'./scripts/autocompletar.js',
    //'./Scripts/jquery-3.3.1.js',


    './Content/Site.css',
    './Content/fontawesome-all.min.css',
    './Content/bootstrap-datetimepicker.css',
    './Content/bootstrap.css',
    './webfonts/fa-solid-900.woff'




    //'./favicon.ico'


];

self.addEventListener('install', (evt) => {

    evt.waitUntil(
        caches.open(CACHE_NAME).then((cache) => {

            return cache.addAll(FILES_TO_CACHE);
        })
    );

    self.skipWaiting();
});

self.addEventListener('activate', (evt) => {

    evt.waitUntil(
        caches.keys().then((keyList) => {
            return Promise.all(keyList.map((key) => {
                if (key !== CACHE_NAME && key !== DATA_CACHE_NAME) {

                    return caches.delete(key);
                }
            }));
        })
    );

    self.clients.claim();
});

self.addEventListener('fetch', (evt) => {


        evt.respondWith(
            caches.open(CACHE_NAME).then((cache) => {
                return cache.match(evt.request)
                    .then((response) => {
                        return response || fetch(evt.request)

                    }).catch(() => {

                        //con esto este fetch no tira mas net::ERR_FAILED y siempre responde 200 devolviendo el offline.html

                        //return caches.open(CACHE_NAME)
                        //    .then((cache) => {
                        //        return cache.match('offline.html');
                        //    });

                    });
            })
        );



});




