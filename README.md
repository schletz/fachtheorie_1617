# Inhalte der POS FT Matura
## Umfang der Aufgaben
Die Datenbank samt Modelklassen wird *vorgegeben*. Als Eigenleistung wird folgendes erwartet:
- Businesslogik auf Basis der Modelklassen schreiben (ergänzen bzw. neue Typen definieren). Dabei sind
Geschäftsfälle vorgegeben, die abgebildet werden sollen.
- Durch Unittests die oben geschriebenen Methoden testen.
- Über ein REST Webservice Daten zugänglich machen. Dabei werden nur Daten ausgelesen und als JSON
angezeigt.

## Erforderliche Technologien in C&#35;
| Technik | Aufgabe |
|---------|---------|
| Entity Framework | Zugriff auf die Datenbank, z. B. `saveChanges`.|
| Web Api 2        | Definieren des REST Webservices, implementieren der Controller.
| Unittests        | Mit Assert die Businesslogik testen. |
| **LINQ**         | Für alle Bereiche. Wichtigste Technologie bei der Umsetzung!
