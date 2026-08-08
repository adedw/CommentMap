import Map from "ol/Map";
import { Tile as TileLayer, Vector as VectorLayer } from "ol/layer";
import { XYZ, Vector as VectorSource } from "ol/source";
import { FullScreen, defaults as defaultControls } from "ol/control";
import { Coordinate } from "ol/coordinate";
import View from "ol/View";
import Feature from "ol/Feature";
import { Point } from "ol/geom";
import { Icon, Style } from "ol/style";

const DEFAULT_ZOOM = 10;

const markerIconStyle = new Style({
  image: new Icon({
    anchor: [0.5, 22],
    anchorXUnits: "fraction",
    anchorYUnits: "pixels",
    src: "/assets/marker.svg",
  }),
});

const root = document.getElementById("root");
if (root) {
  const cards = Array.from(root.querySelectorAll<HTMLElement>("[data-location]"));
  const coordinates: Coordinate[] = cards.map((card) =>
    JSON.parse(card.getAttribute("data-location")!)
  );

  const first = coordinates.length > 0 ? coordinates[0] : [0, 0];

  const map = new Map({
    target: "map",
    layers: [
      new TileLayer({
        source: new XYZ({
          url: "https://mt0.google.com/vt/lyrs=m&hl=en&x={x}&y={y}&z={z}",
        }),
      }),
      new VectorLayer({
        source: new VectorSource({
          features: coordinates.map((coordinate) => {
            const feature = new Feature({
              geometry: new Point(coordinate),
            });
            feature.setStyle(markerIconStyle);
            return feature;
          }),
        }),
      }),
    ],
    view: new View({
      center: first,
      zoom: DEFAULT_ZOOM,
      projection: "EPSG:3857",
    }),
    controls: defaultControls().extend([new FullScreen()]),
  });

  function goToLocation(coordinate: Coordinate) {
    map.getView().setCenter(coordinate);
  }

  root.querySelectorAll<HTMLElement>("[data-goto-location]").forEach((button) => {
    const card = button.closest<HTMLElement>("[data-location]");
    button.addEventListener("click", () => {
      goToLocation(JSON.parse(card!.getAttribute("data-location")!));
    });
  });
}
