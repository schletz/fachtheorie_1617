# Inhalte der POS FT Matura
## Umfang der Aufgaben
Die Datenbank samt Modelklassen wird *vorgegeben*. Als Eigenleistung wird folgendes erwartet:
- Die Angabe beinhaltet Geschäftsfälle, die abzubilden sind. Die Businesslogik dazu ist auf Basis der Modelklassen schreiben (ergänzen bzw. neue Typen definieren). 
- Es ist kein Frontend vorgesehen, da für 5 Stunden die Zeit hierfür nicht ausreichen würde. Die Logik soll jedoch durch Unittests geprüft werden.
- Über ein REST Webservice sollen Daten zugänglich gemacht werden. Dabei werden nur Daten ausgelesen und als JSON angezeigt.


## Erforderliche Technologien in C&#35;
Wer die Aufgabenstellung in C# löst, sollte die Sprachsyntax und die grundlegenden Klassen im .NET Framework kennen:
- OOP Features in C#
- Properties, default Properties (C# 5)
- Collections
- **LINQ**

| Technik | Aufgabe |
|---------|---------|
| Entity Framework | Zugriff auf die Datenbank, z. B. `saveChanges`.|
| Web Api 2        | Definieren des REST Webservices, implementieren der Controller.
| Unittests        | Mit Assert die Businesslogik testen. |
| **LINQ**         | Für alle Bereiche. Wichtigste Technologie bei der Umsetzung!
