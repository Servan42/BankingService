# Banking Service [![package](https://github.com/Servan42/BankingService/actions/workflows/package.yml/badge.svg?branch=master&event=push)](https://github.com/Servan42/BankingService/actions/workflows/package.yml)

## Functionalities

### Import and storage Interface

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

- Implement a way to import new `Type`, `Comment Auto` and `Categorie`

### Dashboard Interface

#### DONE

- Get Sum(€) per categorie (Inputs: Month)
- Get balance +/- (Inputs: Month)
- Get balance +/- (Inputs: Year)
- Get Sum(€) - (Inputs: Month)
- Get Sum(€) + (Inputs: Month)
- Get highest operations (Inputs: Month, minAmount)
- Get `Date,Treasury` list for graph (Inputs: Month)

### Console UI

#### DONE

- Implement a CLI
- Think of a way to fill missing data manually
- Implement a way to import data files.
- Implement a way to review lines that need manual commentary and add new categories, types, and autocomment before recalculating the db.