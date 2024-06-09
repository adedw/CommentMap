import Map from "ol/Map";
import { Tile as TileLayer, Vector as VectorLayer } from "ol/layer";
import { XYZ, Vector as VectorSource } from "ol/source";
import { FullScreen, defaults as defaultControls } from "ol/control";
import { MapViewModel } from "./MapViewModel";
import type { Location } from "./types";

const locations: Location[] = $("[data-location]").map(function () {
  return $(this).data("location");
}).get();

const mapViewModel = MapViewModel.fromLocations(locations);

const defaultView = mapViewModel.getViewAtFirst();

const features = mapViewModel.getPointFeatures();

const map = new Map({
  target: "map",
  layers: [
    new TileLayer({
      source: new XYZ({
        url: "https://mt0.google.com/vt/lyrs=m&hl=en&x={x}&y={y}&z={z}"
      }),
    }),
    new VectorLayer({
      source: new VectorSource({ features })
    })
  ],
  view: defaultView,
  controls: defaultControls().extend([new FullScreen()]),
});
