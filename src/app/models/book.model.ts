export interface Book {
    bookId: string,
    isbn: string,
    title: string,
    description: string,
    genre: string,
    coverUrl: string,
    publisherId: string,
    authorsIds: string[]
  }