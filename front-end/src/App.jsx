import { useState, useEffect } from 'react';
import { Users, ShoppingCart, Plus, Edit2, Trash2, RefreshCw } from 'lucide-react';

const API_BASE_URL_USER = 'http://localhost:5001/api';
const API_BASE_URL_ORDER = 'http://localhost:5066/api';

export default function App() {
  const [activeTab, setActiveTab] = useState('users');
  const [users, setUsers] = useState([]);
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  // User form state
  const [userForm, setUserForm] = useState({ id: null, name: '', email: '' });
  const [showUserForm, setShowUserForm] = useState(false);

  // Order form state
  const [orderForm, setOrderForm] = useState({ id: null, userId: '', productName: '', amount: '', status: 'Pending' });
  const [showOrderForm, setShowOrderForm] = useState(false);

  useEffect(() => {
    if (activeTab === 'users') {
      fetchUsers();
    } else {
      fetchOrders();
    }
  }, [activeTab]);

  // User API calls
  const fetchUsers = async () => {
    setLoading(true);
    setError('');
    try {
      const response = await fetch(`${API_BASE_URL_USER}/Users`);
      if (!response.ok) throw new Error('Failed to fetch users');
      const data = await response.json();
      setUsers(data);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const saveUser = async () => {
    setLoading(true);
    setError('');
    try {
      const url = userForm.id
        ? `${API_BASE_URL_USER}/Users/${userForm.id}`
        : `${API_BASE_URL_USER}/Users`;

      const method = userForm.id ? 'PUT' : 'POST';

      const response = await fetch(url, {
        method,
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ name: userForm.name, email: userForm.email })
      });

      if (!response.ok) throw new Error('Failed to save user');

      await fetchUsers();
      resetUserForm();
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const deleteUser = async (id) => {
    if (!confirm('Are you sure you want to delete this user?')) return;

    setLoading(true);
    setError('');
    try {
      const response = await fetch(`${API_BASE_URL_USER}/Users/${id}`, {
        method: 'DELETE'
      });

      if (!response.ok) throw new Error('Failed to delete user');

      await fetchUsers();
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const editUser = (user) => {
    setUserForm({ id: user.id, name: user.name, email: user.email });
    setShowUserForm(true);
  };

  const resetUserForm = () => {
    setUserForm({ id: null, name: '', email: '' });
    setShowUserForm(false);
  };

  // Order API calls
  const fetchOrders = async () => {
    setLoading(true);
    setError('');
    try {
      const response = await fetch(`${API_BASE_URL_ORDER}/orders`);
      if (!response.ok) throw new Error('Failed to fetch orders');
      const data = await response.json();
      setOrders(data);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const saveOrder = async () => {
    setLoading(true);
    setError('');
    try {
      const url = orderForm.id
        ? `${API_BASE_URL_ORDER}/orders/${orderForm.id}`
        : `${API_BASE_URL_ORDER}/orders`;

      const method = orderForm.id ? 'PUT' : 'POST';

      const body = orderForm.id
        ? { productName: orderForm.productName, amount: parseFloat(orderForm.amount), status: orderForm.status }
        : { userId: parseInt(orderForm.userId), productName: orderForm.productName, amount: parseFloat(orderForm.amount) };

      const response = await fetch(url, {
        method,
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body)
      });

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(errorText || 'Failed to save order');
      }

      await fetchOrders();
      resetOrderForm();
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const deleteOrder = async (id) => {
    if (!confirm('Are you sure you want to delete this order?')) return;

    setLoading(true);
    setError('');
    try {
      const response = await fetch(`${API_BASE_URL_ORDER}/orders/${id}`, {
        method: 'DELETE'
      });

      if (!response.ok) throw new Error('Failed to delete order');

      await fetchOrders();
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const editOrder = (order) => {
    setOrderForm({
      id: order.id,
      userId: order.userId,
      productName: order.productName,
      amount: order.amount,
      status: order.status
    });
    setShowOrderForm(true);
  };

  const resetOrderForm = () => {
    setOrderForm({ id: null, userId: '', productName: '', amount: '', status: 'Pending' });
    setShowOrderForm(false);
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 p-8">
      <div className="max-w-7xl mx-auto">
        <h1 className="text-4xl font-bold text-gray-800 mb-2">Microservices Demo</h1>

        {/* Tabs */}
        <div className="flex gap-2 mb-6">
          <button
            onClick={() => setActiveTab('users')}
            className={`flex items-center gap-2 px-6 py-3 rounded-lg font-medium transition-all ${activeTab === 'users'
              ? 'bg-white text-blue-600 shadow-md'
              : 'bg-white/50 text-gray-600 hover:bg-white/80'
              }`}
          >
            <Users size={20} />
            Users
          </button>
          <button
            onClick={() => setActiveTab('orders')}
            className={`flex items-center gap-2 px-6 py-3 rounded-lg font-medium transition-all ${activeTab === 'orders'
              ? 'bg-white text-blue-600 shadow-md'
              : 'bg-white/50 text-gray-600 hover:bg-white/80'
              }`}
          >
            <ShoppingCart size={20} />
            Orders
          </button>
        </div>

        {/* Error Message */}
        {error && (
          <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-lg mb-4">
            {error}
          </div>
        )}

        {/* Users Tab */}
        {activeTab === 'users' && (
          <div className="bg-white rounded-xl shadow-lg p-6">
            <div className="flex justify-between items-center mb-6">
              <h2 className="text-2xl font-bold text-gray-800">User Management</h2>
              <div className="flex gap-2">
                <button
                  onClick={fetchUsers}
                  disabled={loading}
                  className="flex items-center gap-2 px-4 py-2 bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200 transition disabled:opacity-50"
                >
                  <RefreshCw size={16} className={loading ? 'animate-spin' : ''} />
                  Refresh
                </button>
                <button
                  onClick={() => setShowUserForm(!showUserForm)}
                  className="flex items-center gap-2 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition"
                >
                  <Plus size={16} />
                  Add User
                </button>
              </div>
            </div>

            {/* User Form */}
            {showUserForm && (
              <div className="bg-blue-50 p-4 rounded-lg mb-6">
                <h3 className="font-semibold mb-4">{userForm.id ? 'Edit User' : 'New User'}</h3>
                <div className="grid grid-cols-2 gap-4 mb-4">
                  <input
                    type="text"
                    placeholder="Name"
                    value={userForm.name}
                    onChange={(e) => setUserForm({ ...userForm, name: e.target.value })}
                    className="px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  />
                  <input
                    type="email"
                    placeholder="Email"
                    value={userForm.email}
                    onChange={(e) => setUserForm({ ...userForm, email: e.target.value })}
                    className="px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  />
                </div>
                <div className="flex gap-2">
                  <button
                    onClick={saveUser}
                    disabled={loading}
                    className="px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition disabled:opacity-50"
                  >
                    Save
                  </button>
                  <button
                    onClick={resetUserForm}
                    className="px-6 py-2 bg-gray-300 text-gray-700 rounded-lg hover:bg-gray-400 transition"
                  >
                    Cancel
                  </button>
                </div>
              </div>
            )}

            {/* Users Table */}
            <div className="overflow-x-auto">
              <table className="w-full">
                <thead className="bg-gray-50">
                  <tr>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">ID</th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Name</th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Email</th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Actions</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-gray-200">
                  {users.length === 0 ? (
                    <tr>
                      <td colSpan="4" className="px-6 py-4 text-center text-gray-500">
                        {loading ? 'Loading...' : 'No users found'}
                      </td>
                    </tr>
                  ) : (
                    users.map((user) => (
                      <tr key={user.id} className="hover:bg-gray-50">
                        <td className="px-6 py-4 text-sm text-gray-900">{user.id}</td>
                        <td className="px-6 py-4 text-sm text-gray-900">{user.name}</td>
                        <td className="px-6 py-4 text-sm text-gray-900">{user.email}</td>
                        <td className="px-6 py-4 text-sm">
                          <div className="flex gap-2">
                            <button
                              onClick={() => editUser(user)}
                              className="text-blue-600 hover:text-blue-800"
                            >
                              <Edit2 size={16} />
                            </button>
                            <button
                              onClick={() => deleteUser(user.id)}
                              className="text-red-600 hover:text-red-800"
                            >
                              <Trash2 size={16} />
                            </button>
                          </div>
                        </td>
                      </tr>
                    ))
                  )}
                </tbody>
              </table>
            </div>
          </div>
        )}

        {/* Orders Tab */}
        {activeTab === 'orders' && (
          <div className="bg-white rounded-xl shadow-lg p-6">
            <div className="flex justify-between items-center mb-6">
              <h2 className="text-2xl font-bold text-gray-800">Order Management</h2>
              <div className="flex gap-2">
                <button
                  onClick={fetchOrders}
                  disabled={loading}
                  className="flex items-center gap-2 px-4 py-2 bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200 transition disabled:opacity-50"
                >
                  <RefreshCw size={16} className={loading ? 'animate-spin' : ''} />
                  Refresh
                </button>
                <button
                  onClick={() => setShowOrderForm(!showOrderForm)}
                  className="flex items-center gap-2 px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 transition"
                >
                  <Plus size={16} />
                  Add Order
                </button>
              </div>
            </div>

            {/* Order Form */}
            {showOrderForm && (
              <div className="bg-green-50 p-4 rounded-lg mb-6">
                <h3 className="font-semibold mb-4">{orderForm.id ? 'Edit Order' : 'New Order'}</h3>
                <div className="grid grid-cols-2 gap-4 mb-4">
                  <input
                    type="number"
                    placeholder="User ID"
                    value={orderForm.userId}
                    onChange={(e) => setOrderForm({ ...orderForm, userId: e.target.value })}
                    disabled={orderForm.id !== null}
                    className="px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent disabled:bg-gray-100"
                  />
                  <input
                    type="text"
                    placeholder="Product Name"
                    value={orderForm.productName}
                    onChange={(e) => setOrderForm({ ...orderForm, productName: e.target.value })}
                    className="px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent"
                  />
                  <input
                    type="number"
                    step="0.01"
                    placeholder="Amount"
                    value={orderForm.amount}
                    onChange={(e) => setOrderForm({ ...orderForm, amount: e.target.value })}
                    className="px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent"
                  />
                  {orderForm.id && (
                    <select
                      value={orderForm.status}
                      onChange={(e) => setOrderForm({ ...orderForm, status: e.target.value })}
                      className="px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent"
                    >
                      <option value="Pending">Pending</option>
                      <option value="Processing">Processing</option>
                      <option value="Completed">Completed</option>
                      <option value="Cancelled">Cancelled</option>
                    </select>
                  )}
                </div>
                <div className="flex gap-2">
                  <button
                    onClick={saveOrder}
                    disabled={loading}
                    className="px-6 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 transition disabled:opacity-50"
                  >
                    Save
                  </button>
                  <button
                    onClick={resetOrderForm}
                    className="px-6 py-2 bg-gray-300 text-gray-700 rounded-lg hover:bg-gray-400 transition"
                  >
                    Cancel
                  </button>
                </div>
              </div>
            )}

            {/* Orders Table */}
            <div className="overflow-x-auto">
              <table className="w-full">
                <thead className="bg-gray-50">
                  <tr>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">ID</th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">User</th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Product</th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Amount</th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Status</th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Actions</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-gray-200">
                  {orders.length === 0 ? (
                    <tr>
                      <td colSpan="6" className="px-6 py-4 text-center text-gray-500">
                        {loading ? 'Loading...' : 'No orders found'}
                      </td>
                    </tr>
                  ) : (
                    orders.map((order) => (
                      <tr key={order.id} className="hover:bg-gray-50">
                        <td className="px-6 py-4 text-sm text-gray-900">{order.id}</td>
                        <td className="px-6 py-4 text-sm">
                          <div className="font-medium text-gray-900">{order.userName}</div>
                          <div className="text-gray-500">{order.userEmail}</div>
                        </td>
                        <td className="px-6 py-4 text-sm text-gray-900">{order.productName}</td>
                        <td className="px-6 py-4 text-sm text-gray-900">${order.amount.toFixed(2)}</td>
                        <td className="px-6 py-4 text-sm">
                          <span className={`px-2 py-1 rounded-full text-xs font-medium ${order.status === 'Completed' ? 'bg-green-100 text-green-800' :
                            order.status === 'Pending' ? 'bg-yellow-100 text-yellow-800' :
                              order.status === 'Processing' ? 'bg-blue-100 text-blue-800' :
                                'bg-red-100 text-red-800'
                            }`}>
                            {order.status}
                          </span>
                        </td>
                        <td className="px-6 py-4 text-sm">
                          <div className="flex gap-2">
                            <button
                              onClick={() => editOrder(order)}
                              className="text-blue-600 hover:text-blue-800"
                            >
                              <Edit2 size={16} />
                            </button>
                            <button
                              onClick={() => deleteOrder(order.id)}
                              className="text-red-600 hover:text-red-800"
                            >
                              <Trash2 size={16} />
                            </button>
                          </div>
                        </td>
                      </tr>
                    ))
                  )}
                </tbody>
              </table>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}