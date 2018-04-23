# How to implement custom map projection


This example describes how to implement custom map projection. In this example the <a href="http://paulbourke.net/geometry/transformationprojection/">Hammer-Aitoff</a> projection was implemented.


<h3>Description</h3>

To&nbsp;create a custom projection&nbsp;implement the&nbsp;<a href="https://documentation.devexpress.com/#WPF/clsDevExpressXpfMapIProjectiontopic">IProjection</a> interface and the following methods of the interface.<br />- <a href="https://documentation.devexpress.com/#WPF/DevExpressXpfMapIProjection_GeoPointToMapUnittopic">GeoPointToMapUnit</a>&nbsp;- converts geographic points to internal map units.<br />-&nbsp;<a href="https://documentation.devexpress.com/#WPF/DevExpressXpfMapIProjection_MapUnitToGeoPointtopic">MapUnitToGeoPoint</a> - converts internal map units to geographic points.<br />- <a href="https://documentation.devexpress.com/#WPF/DevExpressXpfMapIProjection_GeoToKilometersSizetopic">GeoToKilometersSize</a>&nbsp;- converts sizes in geographic units into the corresponding size&nbsp;in kilometers.<br />- <a href="https://documentation.devexpress.com/#WPF/DevExpressXpfMapIProjection_KilometersToGeoSizetopic">KilometersToGeoSize</a>&nbsp;- converts sizes in kilometers into the corresponding size&nbsp;in geographical units.

<br/>


