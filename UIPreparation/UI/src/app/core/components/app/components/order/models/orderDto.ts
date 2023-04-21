export class OrderDto{
    id?:number; 
    productName?: string;
    customerName?: string;
    createdUserId?:number; 
    createdDate?:(Date | any); 
    lastUpdatedUserId?:number; 
    lastUpdatedDate?:(Date | any); 
    status:boolean; 
    isDeleted:boolean; 
    customerId?:number; 
    productId?:number; 
    stock?:number; 
}