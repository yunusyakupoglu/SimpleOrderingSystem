import { Component, AfterViewInit, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { AlertifyService } from 'app/core/services/alertify.service';
import { LookUpService } from 'app/core/services/lookUp.service';
import { AuthService } from 'app/core/components/admin/login/services/auth.service';
import { environment } from 'environments/environment';
import { Order } from './models/order';
import { Customer } from '../customer/models/customer';
import { CustomerService } from '../customer/services/customer.service';
import { OrderService } from './services/order.service';
import { Product } from '../product/models/Product';
import { ProductService } from '../product/services/Product.service';
import { OrderDto } from './models/orderDto';

declare var jQuery: any;

@Component({
	selector: 'app-order',
	templateUrl: './order.component.html',
	styleUrls: ['./order.component.scss']
})
export class OrderComponent implements AfterViewInit, OnInit {

	dataSource: MatTableDataSource<any>;
	@ViewChild(MatPaginator) paginator: MatPaginator;
	@ViewChild(MatSort) sort: MatSort;
	displayedColumns: string[] = [
		 'customerName',
		  'productName',
		   'stock',
		    'update',
			 'delete'
			];

	orderList: Order[];
	orderDtoList: OrderDto[];
	order: Order = new Order();
	productList: Product[];
	customerList: Customer[];

	orderAddForm: FormGroup;


	orderId: number;

	constructor(private orderService: OrderService, private lookupService: LookUpService, private alertifyService: AlertifyService, private formBuilder: FormBuilder, private authService: AuthService, private productService: ProductService, private customerService: CustomerService) { }

	ngAfterViewInit(): void {
		this.getOrderDtoList();
	}

	ngOnInit() {
		this.getProducts();
		this.getCustomers();
		this.createOrderAddForm();
	}


	getOrderList() {
		this.orderService.getOrderList().subscribe(data => {
			this.orderList = data;
			this.dataSource = new MatTableDataSource(data);
			this.configDataTable();
		});
	}

		getOrderDtoList() {
		this.orderService.getOrderDtoList().subscribe(data => {
			this.orderDtoList = data;
			this.dataSource = new MatTableDataSource(data);
			this.configDataTable();
			console.log(data, "order data");
		});
	}

	save() {

		if (this.orderAddForm.valid) {
			this.order = Object.assign({}, this.orderAddForm.value)

			if (this.order.id == 0)
				this.addOrder();
			else
				this.updateOrder();
		}

	}

	addOrder() {
		this.order.createdUserId = this.authService.getUserId();
		this.order.lastUpdatedUserId = this.authService.getUserId();
		this.orderService.addOrder(this.order).subscribe(data => {
			this.getOrderList();
			this.order = new Order();
			jQuery('#order').modal('hide');
			this.alertifyService.success(data);
			this.clearFormGroup(this.orderAddForm);

		})

	}

	updateOrder() {
		this.order.lastUpdatedUserId = this.authService.getUserId();
		this.orderService.updateOrder(this.order).subscribe(data => {
			var index = this.orderList.findIndex(x => x.id == this.order.id);
			this.orderList[index] = this.order;
			this.dataSource = new MatTableDataSource(this.orderList);
			this.configDataTable();
			this.order = new Order();
			jQuery('#order').modal('hide');
			this.alertifyService.success(data);
			this.clearFormGroup(this.orderAddForm);

		})

	}

	createOrderAddForm() {
		this.orderAddForm = this.formBuilder.group({
			id: [0],
			createdUserId: [0],
			createdDate: [Date.now],
			lastUpdatedUserId: [0],
			lastUpdatedDate: [Date.now],
			status: [true],
			isDeleted: [false],
			customerId: [0, Validators.required],
			productId: [0, Validators.required],
			stock: [0, Validators.required]
		})
	}

	deleteOrder(orderId: number) {
		this.orderService.deleteOrder(orderId).subscribe(data => {
			this.alertifyService.success(data.toString());
			this.orderList = this.orderList.filter(x => x.id != orderId);
			this.dataSource = new MatTableDataSource(this.orderList);
			this.configDataTable();
		})
	}

	getOrderById(orderId: number) {
		this.clearFormGroup(this.orderAddForm);
		this.orderService.getOrderById(orderId).subscribe(data => {
			this.order = data;
			this.orderAddForm.patchValue(data);
		})
	}


	clearFormGroup(group: FormGroup) {

		group.markAsUntouched();
		group.reset();

		Object.keys(group.controls).forEach(key => {
			group.get(key).setErrors(null);
			if (key == 'id')
				group.get(key).setValue(0);
		});
	}

	checkClaim(claim: string): boolean {
		return this.authService.claimGuard(claim)
	}

	configDataTable(): void {
		this.dataSource.paginator = this.paginator;
		this.dataSource.sort = this.sort;
	}

	applyFilter(event: Event) {
		const filterValue = (event.target as HTMLInputElement).value;
		this.dataSource.filter = filterValue.trim().toLowerCase();

		if (this.dataSource.paginator) {
			this.dataSource.paginator.firstPage();
		}
	}

	getProducts() {
		this.productService.getProductList().subscribe(data => {
			this.productList = data;
		});
	}

	getCustomers() {
		this.customerService.getCustomerList().subscribe(data => {
			this.customerList = data;
		});
	}


}
