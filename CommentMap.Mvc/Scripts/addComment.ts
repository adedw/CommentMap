import Map from "ol/Map";
import { Tile as TileLayer, Vector as VectorLayer } from "ol/layer";
import { XYZ, Vector as VectorSource } from "ol/source";
import { FullScreen, defaults as defaultControls } from "ol/control";
import View from "ol/View";
import Draw, { DrawEvent } from 'ol/interaction/Draw';
import Point from "ol/geom/Point";
import Feature from "ol/Feature";
import { Coordinate } from "ol/coordinate";

const longitude = $("#longitude");
const latitude = $("#latitude");


const source = new VectorSource();

function handleDrawnPoint({ feature }: DrawEvent) {
  source.clear(true);
  const geometry = feature.getGeometry();
  if (geometry instanceof Point) {
    const coordinates = geometry.getCoordinates();
    longitude.val(coordinates[0]);
    latitude.val(coordinates[1]);
  }
}

const vector = new VectorLayer({ source });

const drawInteraction = new Draw({
  source,
  type: "Point"
});
drawInteraction.on("drawend", handleDrawnPoint);

let initialViewCenter: Coordinate = [0, 0];
if (longitude.val() && latitude.val()) {
  initialViewCenter = [Number(longitude.val()), Number(latitude.val())];
  const point = new Point(initialViewCenter);
  const feature = new Feature(point);
  source.addFeature(feature);
}
const map = new Map({
  layers: [
    new TileLayer({
      source: new XYZ({
        url: "https://mt0.google.com/vt/lyrs=m&hl=en&x={x}&y={y}&z={z}",
      }),
    }),
    vector
  ],
  target: "map",
  view: new View({
    center: initialViewCenter,
    zoom: 4,
    projection: "EPSG:3857"
  }),
  controls: defaultControls().extend([new FullScreen()]),
});
map.addInteraction(drawInteraction);
