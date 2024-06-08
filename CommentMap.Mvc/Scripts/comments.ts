import Map from "ol/Map";
import TileLayer from "ol/layer/Tile";
import XYZ from "ol/source/XYZ";
import View from 'ol/View';

const map = new Map({
    target: "map",
    layers: [
        new TileLayer({
            source: new XYZ({
                url: 'http://mt0.google.com/vt/lyrs=m&hl=en&x={x}&y={y}&z={z}'
            }),
        })
    ],
    view: new View({
        center: [0, 0],
        zoom: 2,
    }),
});
