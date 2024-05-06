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
      newTransactions = newTransactions.filter(
        (x) =>
          x.label.includes(search) ||
          x.comment.includes(search) ||
          x.autoComment.includes(search)
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
}
