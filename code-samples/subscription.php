<?php
/**
 * This file contains the model for subscriptions.
 *
 * PHP Version 5
 *
 * @package     Directory
 * @subpackage  Models
 * @author      Randall Loffelmacher <randall@loffelmacher.com>
 */

/**
 * A Model to represent the subscriptions. Two types: annual and six-month.
 *
 */
class Subscription extends AppModel
{
    /**
     * This helps netbeans to autocomplete.
     *
     * @var $this Model 
     */

    public $name = 'Subscription';
    public $displayField = 'name';
    public $actsAs = array('Containable');
    public $virtualFields = 
        array(
            'name'   => "CONCAT(DATE_FORMAT(Subscription.starts,'%b %e %Y'),' — ',DATE_FORMAT(Subscription.ends,'%b %e %Y'))",
            'active' => 'NOW() BETWEEN Subscription.starts AND Subscription.ends',
        );

    /**
    * Looks for a subscription to renew, then looks for a subscriptions
    * Calls Subscription->create() and called by SubscriptionsController::buy()
    * 
    * @param integer $userid The Subscriber buying a subscription.
    *
    * @return  An array of Subscriptions, whose subscription id used to display
    *          the paypal button.
    */
    public function buy($userid) 
    {
        $dt = new DateTime();
        $today = $dt->format('Y-m-d H:i:s');
        $dt->modify('-1 day');
        $yesterday = $dt->format('Y-m-d H:i:s');
        // check for an existing current subscription
        $renew = $this->find('first', array(
            'conditions' => array(
                'subscriber_id' => $userid,
                'NOW() BETWEEN starts AND ends',
                'paid' => true,
            ),
            'contain' => array(),
        ));
        // check for an abandoned subscription
        $abandoned = $this->find('first', array(
            'conditions' => array(
                'subscriber_id' => $userid,
                'paid' => false,
            ),
            'contain' => array(),
        ));
        if (!empty($abandoned) && !empty($abandoned['Subscription'])) {
            $retval = array(0=>$abandoned); // return this sub
        } elseif (!empty($renew) && !empty($renew['Subscription'])) {
            $data = array('Subscription' => array(
                'subscriber_id'	=> $userid,
                'starts' 		=> $renew['Subscription']['ends'],
                'ends'			=> $yesterday,// gets reset by setExpiration()
                'paid'			=> 0,
            ));
            $this->create();
            $this->save($data);
            $retval = $this->find('all', array(
                'conditions' 	=> array(
                    'Subscription.id'	=> $this->getLastInsertID(),
                ),
                'contain'		=> array(),
            ));
        } else {
            $data = array('Subscription' => array(
                'subscriber_id'	=> $userid,
                'starts' 		=> $today,
                'ends'			=> $yesterday, // gets reset by setExpiration()
                'paid'			=> 0,
            ));
            $this->create();
            $this->save($data);
            $retval = $this->find('all', array(
                'conditions' 	=> array(
                    'Subscription.id'	=> $this->getLastInsertID(),
                ),
                'contain'		=> array(),
            ));
        }
        return $retval;
    }

    /**
     * Called in app_controller.php when the PaypalIpn plugin's 
     * notification event is fired. This gets pings many times 
     * by Paypal so we have to ignore some calls.
     *
     * See http://bit.ly/9gos4c for more info on the checks performed here.
     *
     * @param array $txn The Paypal IPN transaction.
     * @param string $subid The subscription id to mark as paid.
     *
     * @return integer Non-zero if failed, zero if succeeded.
     */
    public function markPaid($txn, $subid) 
    {
        if ($subid) {
            // check the amount
            $is_six_month = $txn['InstantPaymentNotification']['payment_gross']
                            == SIX_MONTH_PRICE;
            $is_annual = $txn['InstantPaymentNotification']['payment_gross']
                            == ANNUAL_PRICE;
            if (!$is_six_month && !$is_annual) {
                return 2;
            }
            // check the item-number
            $is_six_month = $txn['InstantPaymentNotification']['item_number']
                                == SIX_MONTH_CODE;
            $is_annual = $txn['InstantPaymentNotification']['item_number']
                                == ANNUAL_CODE;
            if (!$is_six_month && !$is_annual) {
                return 3;
            }
            // check the receiver email
            $correct_account = strtolower(
                $txn['InstantPaymentNotification']['receiver_email'])
                == strtolower(PAYPAL_ACCOUNT);
            if (!$correct_account) {
                return 4;
            }
            $correct_txn_type = strtolower(
                $txn['InstantPaymentNotification']['txn_type'])
                == 'subscr_payment';
            if (!$correct_txn_type) {
                return 5;
            }
            $this->id = $subid;
            // see http://api13.cakephp.org/class/model#method-ModelsaveField
            if ($this->saveField('paid', 1, false)) {
                return 0; // success, return 0
            }
        }
        return 1; // failed, return non-zero
    }
    
    /**
     * This is called from app_controller when the Paypal IPN has been called
     * and the transaction is validated
     *
     * @param id $subid The id of subscription to modify.
     * @param string $subtype A constant for annual/six-month subscription type.
     *
     * @return void
     */
    public function setExpiration($subid, $subtype) 
    {
        $sub = $this->read('id', $subid);
        if ($subtype == ANNUAL_CODE) {
            $this->updateAll(
                array('Subscription.ends'  => 'DATE_ADD(starts, INTERVAL 1 YEAR)'),
                array('Subscription.id = ' => $subid)
            );
            $this->save();
        } elseif ($subtype == SIX_MONTH_CODE) {
            $this->updateAll(
                array('Subscription.ends'  => 'DATE_ADD(starts, INTERVAL 6 MONTH)'),
                array('Subscription.id = ' => $subid)
            );
            $this->save();
        }
    }

    /**
     * Gets the user's active subscription.
     *
     * @param integer $id The subscriber's id.
     *
     * @return array Contains the subscription or empty is no active sub.
     */
    public function currentSubscription($id) 
    {
        $sub = $this->find('first', array(
                'conditions'    => array(
                    'subscriber_id'	=> $id,
                    'paid' 			=> 1
                ),
                'order'			=> array('Subscription.ends DESC')
            )
        );
        return $sub;
    }

    /**
     * Validation for subscriptions
     *
     */
    public $validate = array(
        'subscriber_id' => array(
            'numeric' => array(
                'rule' => array('numeric'),
                'message' => 'Cannot find the associated subscriber',
            ),
            'notempty' => array(
                'rule' => array('notempty'),
                'message' => 'Cannot find the associated subscriber',
            ),
        ),
        'starts' => array(
            'date' => array(
                'rule' => array('date', array('Mdy')),
                'message' => 'Must be a valid date',
            ),
            'notempty' => array(
                'rule' => array('notempty'),
                'message' => 'Start date is required',
            ),
        ),
        'ends' => array(
            'date' => array(
                'rule' => array('date'),
                'message' => 'Must be a valid date',
            ),
            'notempty' => array(
                'rule' => array('notempty'),
                'message' => 'End date is required',
            ),
        ),
        'paid' => array(
            'boolean' => array(
                'rule' => array('boolean'),
            ),
        ),
    );
    
    /**
     * This model is associated with the Subscriber model.
     * 
     */
    public $belongsTo = array(
        'Subscriber' => array(
            'className' => 'Subscriber',
            'foreignKey' => 'subscriber_id',
            'conditions' => '',
            'fields' => '',
            'order' => ''
        )
    );
}
