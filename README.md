# Banking Service

## Functionalities

### Import and storage

- Import CSV files with the CM format
- Import Paypal CSV files to complete the data about the above
- Ignore and log ducplicates in an import report
- Store in the format `Id,Date,Flow,Treasury,Type,Comment,AutoComment,CommentDetails,Categorie,Libellé,Controle`
- Get `Type`, `Comment Auto` and `Categorie` rules from `BankCSVParser`
- Think of a way to fill missing data manually

### Dashboard

- Get Sum(€) per categorie (Inputs: Month)
- Get balance +/- (Inputs: Month)
- Get balance +/- (Inputs: Year)
- Get Sum(€) - (Inputs: Month)
- Get Sum(€) + (Inputs: Month)
- Get highest operations (Inputs: Month, minAmount)
- Get `Date,Treasury` list for graph (Inputs: Month)