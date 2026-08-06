import { Coordinate } from "ol/coordinate";
import Map from "ol/Map";
import { Tile as TileLayer, Vector as VectorLayer } from "ol/layer";
import { XYZ, Vector as VectorSource } from "ol/source";
import { FullScreen, defaults as defaultControls } from "ol/control";
import View from "ol/View";
import Draw, { DrawEvent } from "ol/interaction/Draw";
import Point from "ol/geom/Point";
import Feature from "ol/Feature";

const root = document.getElementById("root");
if (root) {
  const longitudeInput = root.querySelector<HTMLInputElement>("[data-longitude=\"true\"]");
  const latitudeInput = root.querySelector<HTMLInputElement>("[data-latitude=\"true\"]");
  const locale = root.querySelector<HTMLElement>("[data-locale]")?.getAttribute("data-locale") ?? "en";

  if (!longitudeInput || !latitudeInput) {
    throw new Error("Coordinate inputs not found.");
  }

  const longitudeEl = longitudeInput;
  const latitudeEl = latitudeInput;

  const intl = new Intl.NumberFormat(locale, { maximumFractionDigits: 10 });

  let longitude = Number(longitudeEl.value);
  let latitude = Number(latitudeEl.value);

  const vectorSource = new VectorSource();

  const map = new Map({
    target: "map",
    layers: [
      new TileLayer({
        source: new XYZ({
          url: "https://mt0.google.com/vt/lyrs=m&hl=en&x={x}&y={y}&z={z}",
        }),
      }),
      new VectorLayer({
        source: vectorSource,
      }),
    ],
    view: new View({
      center: [longitude, latitude],
      zoom: 4,
      projection: "EPSG:3857",
    }),
    controls: defaultControls().extend([new FullScreen()]),
  });

  function format(value: number): string {
    return intl.format(value).replace(/\s/g, "");
  }

  function updateInput(input: HTMLInputElement, value: number) {
    input.value = format(value);
    input.dispatchEvent(new Event("change"));
  }

  function setCoordinate(coordinates: Coordinate) {
    const [nextLongitude, nextLatitude] = coordinates;
    if (nextLongitude === undefined || nextLatitude === undefined) {
      return;
    }

    longitude = nextLongitude;
    latitude = nextLatitude;

    updateInput(longitudeEl, longitude);
    updateInput(latitudeEl, latitude);

  }

  function setPoint({ feature }: DrawEvent) {
    vectorSource.clear(true);
    const geometry = feature.getGeometry();
    if (geometry instanceof Point) {
      setCoordinate(geometry.getCoordinates());
    }
  }

  function restorePoint() {
    const point = new Point([longitude, latitude]);
    const feature = new Feature(point);
    vectorSource.addFeature(feature);
  }

  const drawInteraction = new Draw({
    source: vectorSource,
    type: "Point",
  });
  drawInteraction.on("drawend", setPoint);
  map.addInteraction(drawInteraction);

  updateInput(longitudeInput, longitude);
  updateInput(latitudeInput, latitude);

  restorePoint();
}
