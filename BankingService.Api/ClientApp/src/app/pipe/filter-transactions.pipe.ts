import { Pipe, PipeTransform } from '@angular/core';
import { Transaction } from '../model/transaction';
import { TransactionFilters } from '../model/transaction-filters';

@Pipe({
  name: 'filterTransactions',
  standalone: true,
})
export class FilterTransactionsPipe implements PipeTransform {
  transform(
    transactions: Transaction[],
    filters?: TransactionFilters
  ): Transaction[] {
    if (filters === undefined) return transactions;

    let newTransactions: Transaction[] = [...transactions];

    const search = filters.search;
    if (search !== undefined && search !== '') {
      const normalizedSearch = this.normalize(search);
      newTransactions = newTransactions.filter(
        (x) =>
          this.normalize(x.label).includes(normalizedSearch) ||
          this.normalize(x.comment).includes(normalizedSearch) ||
          this.normalize(x.autoComment).includes(normalizedSearch)
      );
    }

    if (filters.category !== undefined) {
      newTransactions = newTransactions.filter(
        (x) => x.category === filters.category
      );
    }

    if (filters.type !== undefined) {
      newTransactions = newTransactions.filter((x) => x.type === filters.type);
    }

    if (filters.startDate !== undefined && filters.endDate !== undefined) {
      newTransactions = newTransactions.filter(
        (x) => x.date >= filters.startDate! && x.date <= filters.endDate!
      );
    }

    return newTransactions;
  }

  private normalize (str: string) {
    return str.normalize('NFD').replace(/[\u0300-\u036f]/g, '').toLowerCase();
  }
}
