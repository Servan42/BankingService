import { DataTag } from "./data-tag";
import { Transaction } from "./transaction";

export interface TransactionReport {
  startDate: Date;
  endDate: Date;
  sumPerCategory: Record<string, number>;
  balance: number;
  balanceWithoutSavings: number;
  positiveSum: number;
  negativeSum: number;
  positiveSumWithoutSavings: number;
  negativeSumWithoutSavings: number;
  highestTransactions: Transaction[];
  treasuryGraphData: DataTag[];
}

function generateMockTransaction(id: number): Transaction {
  return {
    id: id,
    date: new Date(), // Generate a random date for simplicity
    flow: Math.random() > 0.5 ? 1 : -1, // Randomly generate positive or negative flow
    treasury: Math.random() * 1000, // Random treasury amount
    label: `Transaction ${id}`,
    type: Math.random() > 0.5 ? 'Type A' : 'Type B', // Randomly generate type
    category: Math.random() > 0.5 ? 'Category A' : 'Category B', // Randomly generate category
    autoComment: 'Auto-generated comment',
    comment: 'Custom comment',
  };
}

// Generate mock data for highestTransactions array
const mockTransactions: Transaction[] = [];
for (let i = 1; i <= 5; i++) {
  mockTransactions.push(generateMockTransaction(i));
}

// Generate mock data for treasuryGraphData array
const mockTreasuryGraphData: DataTag[] = [];
const currentDate = new Date();
for (let i = 0; i < 12; i++) {
  const date = new Date(currentDate.getFullYear(), currentDate.getMonth(), 30-i);
  const value = Math.random() * 1000;
  mockTreasuryGraphData.push({dateTime: date, value: value});
}

// Generate mock data for SumPerCategory object
const mockSumPerCategory: Record<string, number> = {
  'Income': Math.random() * 1000,
  'Epargne': Math.random() * 1000,
  'Cat A': Math.random() * 1000,
  'Charges A': Math.random() * 1000,
  'Charges B': Math.random() * 1000,
  'Cat B': Math.random() * 1000,
};

// Generate mock data for the Report object
export const mockReport: TransactionReport = {
  startDate: new Date(2022, 0, 1),
  endDate: new Date(2022, 11, 31),
  sumPerCategory: mockSumPerCategory,
  balance: Math.random() * 10000,
  balanceWithoutSavings: Math.random() * 10000,
  positiveSum: Math.random() * 1000,
  negativeSum: Math.random() * 1000,
  positiveSumWithoutSavings: Math.random() * 1000,
  negativeSumWithoutSavings: Math.random() * 1000,
  highestTransactions: mockTransactions,
  treasuryGraphData: mockTreasuryGraphData,
};
