import { Component, AfterViewInit, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { AlertifyService } from 'app/core/services/alertify.service';
import { LookUpService } from 'app/core/services/lookUp.service';
import { AuthService } from 'app/core/components/admin/login/services/auth.service';
import { Product } from './models/Product';
import { ProductService } from './services/Product.service';
import { environment } from 'environments/environment';
import { ESize, SizeMapping } from './models/size';
import { LookUp } from 'app/core/models/lookUp';
import { Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
declare var jQuery: any;

@Component({
	selector: 'app-product',
	templateUrl: './product.component.html',
	styleUrls: ['./product.component.scss']
})


export class ProductComponent implements AfterViewInit, OnInit {

	dataSource: MatTableDataSource<any>;
	@ViewChild(MatPaginator) paginator: MatPaginator;
	@ViewChild(MatSort) sort: MatSort;
	displayedColumns: string[] = ['name', 'color', 'size', 'update', 'delete'];
	sizesLookUp: LookUp[] = [];
	sizeNames: string[] = Object.keys(SizeMapping);
	selectedValue: LookUp;
	sizeControl = new FormControl();
	filteredOptions: Observable<LookUp[]>;
	productList: Product[];
	product: Product = new Product();
	eSize: ESize;
	productAddForm: FormGroup;
	productId: number;

	constructor(private productService: ProductService, private lookupService: LookUpService, private alertifyService: AlertifyService, private formBuilder: FormBuilder, private authService: AuthService) { }

	ngAfterViewInit(): void {
		this.getProductList();
	}

	ngOnInit() {
		this.getSizesEnumsLookUp();
		this.createProductAddForm();
	}

	getSizesEnumsLookUp() {
		this.sizeNames.forEach(element => {
			this.sizesLookUp.push({ id: Number(element), label: SizeMapping[Number(element)] });
		});
		this.filteredOptions = this.sizeControl.valueChanges.pipe(
			startWith(''),
			map(value => {
				const name = typeof value === 'string' ? value : value?.name;
				return name ? this._filter(name as string) : this.sizesLookUp.slice();
			}),
		);
	}

	private _filter(value: string): LookUp[] {
		const filterValue = value.toLowerCase();

		return this.sizesLookUp.filter(option => option.label.toLowerCase().includes(filterValue));
	}

	displayFn(data: LookUp): string {
		return data && data.label ? data.label : '';
	}


	getProductList() {
		this.productService.getProductList().subscribe(data => {
			this.productList = data;
			this.dataSource = new MatTableDataSource(data);
			this.configDataTable();
		});
	}

	save() {
		this.productAddForm.controls.size.setValue(this.sizeControl.value.label);

		if (this.productAddForm.valid) {
			this.product = Object.assign({}, this.productAddForm.value)

			if (this.product.id == 0)
				this.addProduct();
			else
				this.updateProduct();
		}
	}

	addProduct() {
		this.product.createdUserId = this.authService.getUserId();
		this.product.lastUpdatedUserId = this.authService.getUserId();
		this.productService.addProduct(this.product).subscribe(data => {
			this.getProductList();
			this.product = new Product();
			jQuery('#product').modal('hide');
			this.alertifyService.success(data);
			this.clearFormGroup(this.productAddForm);

		},error =>{
			this.alertifyService.error("Bu ürün veritabanında mevcut.");

		})

	}

	updateProduct() {
		this.product.lastUpdatedUserId = this.authService.getUserId();
		this.productService.updateProduct(this.product).subscribe(data => {
			var index = this.productList.findIndex(x => x.id == this.product.id);
			this.productList[index] = this.product;
			this.dataSource = new MatTableDataSource(this.productList);
			this.configDataTable();
			this.product = new Product();
			jQuery('#product').modal('hide');
			this.alertifyService.success(data);
			this.clearFormGroup(this.productAddForm);

		})

	}

	createProductAddForm() {
		this.productAddForm = this.formBuilder.group({
			id: [0],
			createdUserId: [0],
			lastUpdatedUserId: [0],
			status: [true],
			name: ["", Validators.required],
			color: ["", Validators.required],
			size: ["", Validators.required]
		})
	}

	deleteProduct(productId: number) {
		this.productService.deleteProduct(productId).subscribe(data => {
			this.alertifyService.success(data.toString());
			this.productList = this.productList.filter(x => x.id != productId);
			this.dataSource = new MatTableDataSource(this.productList);
			this.configDataTable();
		})
	}

	getProductById(productId: number) {
		debugger
		this.clearFormGroup(this.productAddForm);
		this.productService.getProductById(productId).subscribe(data => {
			this.product = data;

			// Set selected size in mat-autocomplete
			this.sizeControl.setValue(this.sizesLookUp.find(x => x.label == data.size));
			this.productAddForm.patchValue(data);
		})
	}


	clearFormGroup(group: FormGroup) {

		group.markAsUntouched();
		group.reset();

		Object.keys(group.controls).forEach(key => {
			group.get(key).setErrors(null);
			if (key == 'id')
				group.get(key).setValue(0);
			if (key == 'name')
				group.get(key).setValue("");
			if (key == 'color')
				group.get(key).setValue("");
			if (key == 'size')
				group.get(key).setValue("");
				// group.get(key).reset();
		});
		this.sizeControl.setValue("");
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

}
