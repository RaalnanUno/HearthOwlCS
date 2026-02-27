Below is a **single self-contained HTML page** that keeps your UI/Leaflet behavior, but **loads pins from a SharePoint List** using the SharePoint REST API.

It’s designed for the easiest path: **this page is hosted inside the same SharePoint site** (so auth is automatic via your existing browser session/cookies). If you host it *outside* SharePoint, you’ll need OAuth / app registration (note at the end).

---

## 1) Create the SharePoint List (recommended structure)

Create a **Custom List** named:

* **MapPins**

Add these columns (use these *exact names* to keep the code simple):

* **Latitude** (Number)
* **Longitude** (Number)
* **PinDescription** (Multiple lines of text)
* **PinColor** (Choice) – choices like: `blue`, `red`, `green`, `orange`, `purple`, `black`

SharePoint already includes the **Title** column—use that as the pin name.

---

## 2) SharePoint REST URL pattern (what you forgot)

To read list items:

* `https://<tenant>.sharepoint.com/sites/<SiteName>/_api/web/lists/getbytitle('MapPins')/items?...` ([Microsoft Learn][1])

You typically shape the response with OData:

* `.../items?$select=Id,Title,Latitude,Longitude,PinDescription,PinColor&$top=5000` ([Microsoft Learn][1])

---

## 3) Full HTML page (SharePoint List → pins)

> **Change only these two constants**:
>
> * `SITE_URL`
> * `LIST_TITLE`

```html
<!doctype html>
<html lang="en">
  <head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Map Pins - Leaflet + SharePoint List</title>

    <!-- Bootstrap (CSS) -->
    <link
      href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css"
      rel="stylesheet"
    />

    <!-- Bootstrap Icons (needed for bi-geo-alt-fill pin icon) -->
    <link
      rel="stylesheet"
      href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css"
    />

    <!-- Leaflet (CSS) -->
    <link
      rel="stylesheet"
      href="https://unpkg.com/leaflet@1.9.4/dist/leaflet.css"
      integrity="sha256-p4NxAoJBhIIN+hmNHrzRCf9tD/miZyoHS5obTRR9BMY="
      crossorigin=""
    />

    <style>
      body { background: #f8f9fa; }
      #map {
        height: 70vh;
        min-height: 420px;
        border-radius: 0.75rem;
      }
      .loc-list { max-height: 70vh; overflow: auto; }
      .loc-item { cursor: pointer; user-select: none; }
      .loc-item:hover { background: #f1f3f5; }
      .badge-coords {
        font-family: ui-monospace, SFMono-Regular, Menlo, Monaco, Consolas,
          "Liberation Mono", "Courier New", monospace;
      }
      .status-line { min-height: 1.25rem; }
    </style>
  </head>

  <body>
    <div class="container py-4">
      <div class="d-flex align-items-center justify-content-between mb-3">
        <div>
          <h1 class="h4 mb-1">Map Pinpoints (SharePoint List)</h1>
          <div class="text-muted small">
            Leaflet + OpenStreetMap tiles • Pins loaded from SharePoint REST
          </div>
          <div id="status" class="text-muted small status-line"></div>
        </div>

        <div class="d-flex gap-2">
          <button id="btnReload" class="btn btn-outline-secondary btn-sm">
            Reload
          </button>
          <button id="btnFit" class="btn btn-outline-primary btn-sm">
            Fit to pins
          </button>
        </div>
      </div>

      <div class="row g-3">
        <div class="col-12 col-lg-8">
          <div class="card shadow-sm">
            <div class="card-body">
              <div id="map"></div>
            </div>
          </div>
        </div>

        <div class="col-12 col-lg-4">
          <div class="card shadow-sm">
            <div class="card-body">
              <div class="d-flex align-items-center justify-content-between mb-2">
                <div class="fw-semibold">Locations</div>
                <span class="badge text-bg-secondary" id="locCount">0</span>
              </div>

              <input
                id="txtFilter"
                class="form-control form-control-sm mb-3"
                placeholder="Filter by name/description..."
              />

              <div id="locList" class="list-group loc-list"></div>

              <div class="text-muted small mt-3">
                SharePoint columns expected: <code>Title</code>, <code>Latitude</code>,
                <code>Longitude</code>, <code>PinDescription</code>, <code>PinColor</code>.
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="card shadow-sm mt-3">
        <div class="card-body">
          <div class="fw-semibold mb-2">Raw JSON (from SharePoint)</div>
          <pre class="mb-0"><code id="jsonPreview"></code></pre>
        </div>
      </div>
    </div>

    <!-- jQuery -->
    <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>

    <!-- Bootstrap (JS) -->
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>

    <!-- Leaflet (JS) -->
    <script
      src="https://unpkg.com/leaflet@1.9.4/dist/leaflet.js"
      integrity="sha256-20nQCchB9co0qIjJZRGuk2/Z9VM+kNiyxNV1lvTlZBo="
      crossorigin=""
    ></script>

    <script>
      // ============================================================
      // CONFIG
      // ============================================================
      // If this page is hosted inside the SAME SharePoint site, you can usually
      // leave SITE_URL = "" and it will use the current site.
      //
      // Examples:
      //   const SITE_URL = ""; // current site
      //   const SITE_URL = "https://tenant.sharepoint.com/sites/EVAuto";
      const SITE_URL = "";

      const LIST_TITLE = "MapPins";

      // ============================================================
      // SharePoint REST helpers
      // ============================================================
      function spApiBase() {
        // When hosted in SharePoint: relative URLs work great.
        return (SITE_URL || "").replace(/\/$/, "") + "/_api";
      }

      function setStatus(text, isError) {
        $("#status")
          .text(text || "")
          .toggleClass("text-danger", !!isError)
          .toggleClass("text-muted", !isError);
      }

      // Read list items using SharePoint REST:
      //   /_api/web/lists/getbytitle('MapPins')/items?$select=...&$top=...
      // This URL shape is standard for SharePoint REST list reads. :contentReference[oaicite:2]{index=2}
      async function fetchAllListItems() {
        const select =
          "Id,Title,Latitude,Longitude,PinDescription,PinColor";

        let url =
          `${spApiBase()}/web/lists/getbytitle('${encodeURIComponent(LIST_TITLE)}')/items` +
          `?$select=${select}&$top=5000`;

        // Use JSON light, minimal payload. :contentReference[oaicite:3]{index=3}
        const headers = {
          Accept: "application/json;odata=nometadata",
        };

        const all = [];
        while (url) {
          const res = await fetch(url, { headers, credentials: "same-origin" });
          if (!res.ok) {
            const body = await res.text().catch(() => "");
            throw new Error(
              `SharePoint request failed (${res.status} ${res.statusText}). ${body}`
            );
          }

          const data = await res.json();

          // JSON light usually returns: { value: [...] }
          const items = Array.isArray(data?.value) ? data.value : [];
          all.push(...items);

          // Paging: @odata.nextLink may appear if there are more items.
          url = data?.["@odata.nextLink"] || null;
        }

        return all;
      }

      function toLocations(spItems) {
        // Normalize SharePoint items -> your app’s structure
        // We keep 'id' aligned to SharePoint's Id
        return (spItems || [])
          .map((it) => {
            const lat = Number(it.Latitude);
            const lng = Number(it.Longitude);

            return {
              id: it.Id,
              name: it.Title ?? `(Item ${it.Id})`,
              lat,
              lng,
              description: it.PinDescription ?? "",
              color: (it.PinColor || "").toString().trim().toLowerCase() || "blue",
            };
          })
          // filter out bad coordinates
          .filter((l) => Number.isFinite(l.lat) && Number.isFinite(l.lng));
      }

      // ============================================================
      // MAP INIT
      // ============================================================
      const map = L.map("map", {
        zoomControl: true,
        scrollWheelZoom: true,
      });

      // OpenStreetMap tile layer (no API key)
      L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
        maxZoom: 19,
        attribution: "&copy; OpenStreetMap contributors",
      }).addTo(map);

      // Marker storage (id -> marker)
      const markersById = new Map();

      // Bounds (must be LET because we reassign it)
      let bounds = L.latLngBounds([]);

      // Optional: icon pins (Bootstrap Icon). If you prefer circle markers, set USE_ICON_PINS=false.
      const USE_ICON_PINS = true;

      function colorToBootstrapTextClass(color) {
        // Basic mapping; expand as you like.
        switch ((color || "").toLowerCase()) {
          case "red":
            return "text-danger";
          case "green":
            return "text-success";
          case "orange":
          case "yellow":
            return "text-warning";
          case "blue":
            return "text-primary";
          case "black":
          case "gray":
          case "grey":
            return "text-dark";
          case "purple":
            return "text-purple"; // not a default class; will fall back visually
          default:
            return "text-primary";
        }
      }

      function makePinIcon(color) {
        const colorClass = colorToBootstrapTextClass(color);
        return L.divIcon({
          className: "",
          iconSize: [24, 24],
          iconAnchor: [12, 24],
          popupAnchor: [0, -24],
          html: `<i class="bi bi-geo-alt-fill ${colorClass}" style="font-size:24px; text-shadow:0 1px 2px rgba(0,0,0,.35);"></i>`,
        });
      }

      function buildMarkers(data) {
        // remove existing markers
        markersById.forEach((m) => map.removeLayer(m));
        markersById.clear();

        // reset bounds
        bounds = L.latLngBounds([]);

        data.forEach((loc) => {
          let marker;

          if (USE_ICON_PINS) {
            marker = L.marker([loc.lat, loc.lng], {
              icon: makePinIcon(loc.color),
            });
          } else {
            marker = L.circleMarker([loc.lat, loc.lng], {
              radius: 8,
              color: loc.color || "blue",
              weight: 2,
              fillColor: loc.color || "blue",
              fillOpacity: 0.9,
            });
          }

          marker
            .addTo(map)
            .bindPopup(`
              <div style="min-width: 220px;">
                <div class="fw-semibold">${escapeHtml(loc.name)}</div>
                <div class="text-muted small mb-2">
                  <span class="badge text-bg-light badge-coords">
                    ${loc.lat.toFixed(4)}, ${loc.lng.toFixed(4)}
                  </span>
                </div>
                ${
                  loc.description
                    ? `<div>${escapeHtml(loc.description)}</div>`
                    : ""
                }
              </div>
            `);

          markersById.set(loc.id, marker);
          bounds.extend([loc.lat, loc.lng]);
        });
      }

      function fitToPins() {
        if (bounds.isValid()) {
          map.fitBounds(bounds.pad(0.2));
        } else {
          map.setView([39.5, -98.35], 4); // USA-ish default
        }
      }

      // ============================================================
      // LIST UI
      // ============================================================
      let locations = [];

      function renderList(data) {
        $("#locCount").text(data.length);
        const $list = $("#locList").empty();

        if (data.length === 0) {
          $list.append(`<div class="text-muted small">No matches.</div>`);
          return;
        }

        data.forEach((loc) => {
          const $item = $(`
            <div class="list-group-item loc-item">
              <div class="d-flex align-items-start justify-content-between">
                <div>
                  <div class="fw-semibold">${escapeHtml(loc.name)}</div>
                  <div class="text-muted small">${loc.lat.toFixed(4)}, ${loc.lng.toFixed(4)}</div>
                </div>
                <span class="badge text-bg-primary">Pin</span>
              </div>
              ${
                loc.description
                  ? `<div class="small mt-1">${escapeHtml(loc.description)}</div>`
                  : ""
              }
            </div>
          `);

          $item.on("click", () => {
            const marker = markersById.get(loc.id);
            if (!marker) return;

            const latlng =
              marker.getLatLng ? marker.getLatLng() : marker._latlng;

            map.setView(latlng, Math.max(map.getZoom(), 12), { animate: true });
            marker.openPopup();
          });

          $list.append($item);
        });
      }

      function applyFilter() {
        const term = ($("#txtFilter").val() || "")
          .toString()
          .trim()
          .toLowerCase();

        if (!term) return locations;

        return locations.filter(
          (l) =>
            (l.name || "").toLowerCase().includes(term) ||
            (l.description || "").toLowerCase().includes(term)
        );
      }

      // ============================================================
      // SAFETY: basic HTML escape for popup/list text
      // ============================================================
      function escapeHtml(str) {
        return (str ?? "")
          .toString()
          .replaceAll("&", "&amp;")
          .replaceAll("<", "&lt;")
          .replaceAll(">", "&gt;")
          .replaceAll('"', "&quot;")
          .replaceAll("'", "&#039;");
      }

      // ============================================================
      // BOOT / LOAD
      // ============================================================
      async function loadFromSharePoint() {
        setStatus("Loading pins from SharePoint…", false);

        try {
          const spItems = await fetchAllListItems();
          locations = toLocations(spItems);

          $("#jsonPreview").text(JSON.stringify(spItems, null, 2));

          buildMarkers(locations);
          fitToPins();
          renderList(locations);

          setStatus(`Loaded ${locations.length} pin(s) from list "${LIST_TITLE}".`, false);
        } catch (err) {
          console.error(err);
          setStatus(
            "Failed to load from SharePoint. Open DevTools Console for details.",
            true
          );

          // Show details in JSON preview so you can see what's happening.
          $("#jsonPreview").text(String(err?.message || err));
          locations = [];
          buildMarkers([]);
          fitToPins();
          renderList([]);
        }
      }

      $(function () {
        // initial view while loading
        map.setView([39.5, -98.35], 4);

        $("#txtFilter").on("input", () => {
          const filtered = applyFilter();
          buildMarkers(filtered);
          fitToPins();
          renderList(filtered);
        });

        $("#btnFit").on("click", fitToPins);
        $("#btnReload").on("click", loadFromSharePoint);

        loadFromSharePoint();
      });
    </script>
  </body>
</html>
```

---

## Notes that will save you pain

### Hosting location matters

* **If you host this page inside the same SharePoint site**, `fetch(..., credentials:"same-origin")` will work using your current login session.
* **If you host it outside SharePoint (localhost, your own site, etc.)**, SharePoint will block it via auth/CORS and you’ll need OAuth (Entra ID app + tokens). The page above is intentionally the “easy” in-SharePoint route.

### Creating items / updates later

Reading (GET) is easy. If later you want to **POST/UPDATE/DELETE** list items, SharePoint REST requires a valid request digest (`X-RequestDigest`) for non-GET calls. ([Microsoft Learn][2])

---

If you tell me **where** you plan to host this (SharePoint page, Script Editor, SPFx, or external site), I’ll adapt the exact approach (auth strategy + deployment style) so it’s “copy/paste and done.”

[1]: https://learn.microsoft.com/en-us/sharepoint/dev/sp-add-ins/working-with-lists-and-list-items-with-rest?utm_source=chatgpt.com "Working with lists and list items with REST"
[2]: https://learn.microsoft.com/en-us/sharepoint/dev/spfx/web-parts/basics/working-with-requestdigest?utm_source=chatgpt.com "Work with __REQUESTDIGEST"
