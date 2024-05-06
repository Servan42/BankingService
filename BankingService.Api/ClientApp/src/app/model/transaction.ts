export interface Transaction {
  id: number;
  date: Date;
  flow: number;
  treasury: number;
  label: string;
  type: string;
  category: string;
  autoComment: string;
  comment: string;
}

export const mockTransactions: Transaction[] = [
  {
    id: 1,
    date: new Date('2024-05-01'),
    flow: -500,
    treasury: 5000,
    label: "Grocery Shopping",
    type: "Expense",
    category: "Food",
    autoComment: "Transaction for weekly groceries",
    comment: "Bought groceries for the week"
  },
  {
    id: 2,
    date: new Date('2024-05-02'),
    flow: 1000,
    treasury: 6000,
    label: "Salary Deposit",
    type: "Income",
    category: "Salary",
    autoComment: "Monthly salary deposit",
    comment: "Received monthly salary"
  },
  {
    id: 3,
    date: new Date('2024-05-03'),
    flow: -50,
    treasury: 5950,
    label: "Dinner with Friends",
    type: "Expense",
    category: "Entertainment",
    autoComment: "Spent on dinner with friends",
    comment: "Enjoyed dinner with friends at a local restaurant"
  },
  {
    id: 4,
    date: new Date('2024-05-04'),
    flow: -200,
    treasury: 5750,
    label: "Utility Bill Payment",
    type: "Expense",
    category: "Utilities",
    autoComment: "Monthly utility bill payment",
    comment: "Paid electricity and water bills for the month"
  },
  {
    id: 5,
    date: new Date('2024-05-05'),
    flow: 300,
    treasury: 6050,
    label: "Freelance Project Payment",
    type: "Income",
    category: "Freelance",
    autoComment: "Payment received for freelance project",
    comment: "Completed and delivered freelance project"
  }
];
