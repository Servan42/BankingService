# Banking Service

## Functionalities

### Import and storage

#### DONE

- Import CSV files with the CM format
- Ignore duplicate on import
- Store in the format `Date;Flow;Treasury;Label;Type;Category;AutoComment;Comment`
- Get `Type`, `Comment Auto` and `Categorie` rules from `BankCSVParser`
- Import Paypal CSV files to complete the data about the above
	- Get `Net` when less than 0
	- Get `Nom`
- Log ducplicates in an import report
- Implement a way to update all existing lines with `Type`, `Comment Auto` and `Categorie` when this data is updated

#### TODO

- Think of a way to fill missing data manually
- Make a specific tables for categories
- Implement a way to import new `Type`, `Comment Auto` and `Categorie`

### Dashboard

- Get Sum(€) per categorie (Inputs: Month)
- Get balance +/- (Inputs: Month)
- Get balance +/- (Inputs: Year)
- Get Sum(€) - (Inputs: Month)
- Get Sum(€) + (Inputs: Month)
- Get highest operations (Inputs: Month, minAmount)
- Get `Date,Treasury` list for graph (Inputs: Month)