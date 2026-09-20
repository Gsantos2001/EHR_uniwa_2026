# EHR ICU Clinical Simulation

Εκπαιδευτική εφαρμογή προσομοίωσης Μονάδας Εντατικής Θεραπείας (ΜΕΘ), υλοποιημένη σε Unity.

Ο χρήστης αλληλεπιδρά με ασθενή και ιατρικό εξοπλισμό, παρακολουθεί ζωτικά σημεία, πραγματοποιεί κλινικές παρεμβάσεις και καταγράφει τις ενέργειές του σε ηλεκτρονικό φάκελο ασθενούς (EHR).

## Βασικά χαρακτηριστικά

- Διαδραστικό ICU περιβάλλον
- Scenario Engine με δυναμική φόρτωση JSON
- Branching αποφάσεις και scoring
- Παρακολούθηση ζωτικών σημείων
- EHR με Assessment, Intervention και Communication
- Documentation Gates
- Hypoxia alarm
- Online Help
- Pause Menu και έλεγχος έντασης ήχου
- Logging ενεργειών και export σε JSON
- Τελικό Debrief με score, checklist και decision path

## Scenario

Το βασικό scenario φορτώνεται δυναμικά στο unity editor από:

```text
Assets/StreamingAssets/icu_scenario.json
```
Στο build βρίσκεται:
```text
EHR_uniwa_2026_Data\StreamingAssets/icu_scenario.json
```
Το build αποθηκεύει τα τελικά json logs στον φάκελο
```text
EHR_uniwa_2026_Data\ScenarioLogs
```
# Μέλη Ομάδας (Ονοματεπώνυμο και ΑΜ)
Γρηγόριος Στρατάκιας 22390281  
Γεώργιος Σάντος 20390204  
Ελευθέριος Ξένος 18390093  
Μανουέλ Γκιόλλα 22390038  
Αλκιβιάδης Φάσσος Κοντοδημάκης 713242017065  
Φίλιππος Στρίντζης 22390215  

